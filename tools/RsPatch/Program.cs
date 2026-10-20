using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

// Regard and Standing, isteğe bağlı Calradia Remembers'la konuşan kod taşıyor ve o modun DLL'lerine referans veriyor
// (CalradiaRemembers, CalradiaRemembers.Logic, NpcMemoryService.Core). Calradia Remembers kurulu değilken oyun bu referansları
// çözemiyor ve her açılışta "submodule could not be loaded correctly due to a dependency conflict" penceresini gösteriyor.
// Bu araç o bağlantıyı DLL'den tamamen söker:
//   - imzasında (dönüş, parametre) o DLL'lerin türleri geçen metotlar ve o türden alanlar, özellikler, olaylar silinir;
//   - gövdesinde o türler geçen metotların gövdesi boşaltılır (varsayılan değer döner); mod bunları zaten yalnızca
//     Calradia Remembers varken çağırıyor;
//   - o türlerden alınan arayüzler sökülür, sonra DLL referansları silinir.
// Modun geri kalanına dokunulmaz. Kullanım: RsPatch <RegardStanding.dll>  (yerinde değiştirir; ikinci kez çalıştırmak bir şey yapmaz)
static class Program
{
    static readonly string[] Optional = { "CalradiaRemembers", "CalradiaRemembers.Logic", "NpcMemoryService.Core" };

    static bool BadScope(IMetadataScope scope) => scope is AssemblyNameReference a && Optional.Contains(a.Name);

    static bool Bad(TypeReference t)
    {
        switch (t)
        {
            case null: return false;
            case GenericInstanceType g: return Bad(g.ElementType) || g.GenericArguments.Any(Bad);
            case FunctionPointerType f: return Bad(f.ReturnType) || f.Parameters.Any(p => Bad(p.ParameterType));
            case TypeSpecification s: return Bad(s.ElementType);
            case GenericParameter _: return false;
        }
        if (t.DeclaringType != null && Bad(t.DeclaringType)) return true;
        return BadScope(t.Scope);
    }

    static bool BadSignature(MethodReference m) =>
        Bad(m.ReturnType) || m.Parameters.Any(p => Bad(p.ParameterType))
        || (m is GenericInstanceMethod g && g.GenericArguments.Any(Bad));

    static bool BadOperand(object operand)
    {
        switch (operand)
        {
            case TypeReference t: return Bad(t);
            case MethodReference m: return Bad(m.DeclaringType) || BadSignature(m)
                || (m is GenericInstanceMethod g && Bad(g.ElementMethod.DeclaringType));
            case FieldReference f: return Bad(f.DeclaringType) || Bad(f.FieldType);
            case CallSite c: return Bad(c.ReturnType) || c.Parameters.Any(p => Bad(p.ParameterType));
        }
        return false;
    }

    static bool BadAttributes(ICustomAttributeProvider p) =>
        p.HasCustomAttributes && p.CustomAttributes.Any(a => Bad(a.AttributeType) || BadSignature(a.Constructor)
            || a.ConstructorArguments.Any(x => Bad(x.Type) || (x.Value is TypeReference tr && Bad(tr))));

    static int Main(string[] args)
    {
        if (args.Length != 1 || !File.Exists(args[0])) { Console.WriteLine("Kullanım: RsPatch <RegardStanding.dll>"); return 2; }
        string path = args[0];
        var module = ModuleDefinition.ReadModule(new MemoryStream(File.ReadAllBytes(path)));
        if (!module.AssemblyReferences.Any(a => Optional.Contains(a.Name))) { Console.WriteLine("Zaten yamalı: " + path); return 0; }

        int removed = 0, emptied = 0;
        List<TypeDefinition> types = module.GetTypes().ToList();
        foreach (TypeDefinition type in types)
        {
            if (Bad(type.BaseType)) { Console.WriteLine("Üst türü Calradia Remembers'ta: " + type.FullName + " — araç güncellenmeli"); return 1; }
            foreach (InterfaceImplementation i in type.Interfaces.Where(i => Bad(i.InterfaceType)).ToList()) type.Interfaces.Remove(i);
            foreach (FieldDefinition f in type.Fields.Where(f => Bad(f.FieldType) || BadAttributes(f)).ToList()) { type.Fields.Remove(f); removed++; }
            foreach (PropertyDefinition p in type.Properties.Where(p => Bad(p.PropertyType)).ToList()) type.Properties.Remove(p);
            foreach (EventDefinition e in type.Events.Where(e => Bad(e.EventType)).ToList()) type.Events.Remove(e);
            foreach (MethodDefinition m in type.Methods.Where(m => BadSignature(m) || BadAttributes(m)
                || m.Overrides.Any(o => Bad(o.DeclaringType))).ToList())
            {
                foreach (PropertyDefinition p in type.Properties.ToList()) if (p.GetMethod == m || p.SetMethod == m) type.Properties.Remove(p);
                foreach (EventDefinition e in type.Events.ToList()) if (e.AddMethod == m || e.RemoveMethod == m) type.Events.Remove(e);
                type.Methods.Remove(m);
                removed++;
            }
            foreach (MethodDefinition m in type.Methods.Where(m => m.HasBody).ToList())
            {
                MethodBody body = m.Body;
                bool bad = body.Instructions.Any(i => BadOperand(i.Operand)) || body.Variables.Any(v => Bad(v.VariableType))
                    || body.ExceptionHandlers.Any(h => Bad(h.CatchType));
                if (!bad) continue;
                Empty(m);
                emptied++;
            }
        }

        foreach (AssemblyNameReference reference in module.AssemblyReferences.Where(a => Optional.Contains(a.Name)).ToList())
            module.AssemblyReferences.Remove(reference);
        // Cecil üst veriyi yazarken yeniden kurar: yazılan DLL'i yeniden okuyup silinen DLL'lere hiçbir referans kalmadığını doğrula.
        var output = new MemoryStream();
        module.Write(output);
        var written = ModuleDefinition.ReadModule(new MemoryStream(output.ToArray()));
        List<string> left = written.AssemblyReferences.Where(a => Optional.Contains(a.Name)).Select(a => a.Name)
            .Concat(written.GetTypeReferences().Where(Bad).Select(t => t.FullName)).Distinct().ToList();
        if (left.Count > 0) { Console.WriteLine("Kalan referanslar: " + string.Join(", ", left)); return 1; }
        File.WriteAllBytes(path, output.ToArray());
        Console.WriteLine("Yamandı: " + path + " (" + removed + " üye silindi, " + emptied + " metot boşaltıldı)");
        return 0;
    }

    /// <summary>Gövdeyi "varsayılan değeri döndür" ile değiştirir (out parametrelere de varsayılan yazar).</summary>
    static void Empty(MethodDefinition m)
    {
        MethodBody body = m.Body;
        body.Instructions.Clear();
        body.Variables.Clear();
        body.ExceptionHandlers.Clear();
        ILProcessor il = body.GetILProcessor();
        foreach (ParameterDefinition p in m.Parameters.Where(p => p.IsOut && p.ParameterType is ByReferenceType))
        {
            il.Emit(OpCodes.Ldarg, p);
            il.Emit(OpCodes.Initobj, ((ByReferenceType)p.ParameterType).ElementType);
        }
        TypeReference ret = m.ReturnType;
        if (ret.MetadataType != MetadataType.Void)
        {
            if (ret.IsValueType || ret.IsGenericParameter)
            {
                var v = new VariableDefinition(ret);
                body.Variables.Add(v);
                body.InitLocals = true;
                il.Emit(OpCodes.Ldloca, v);
                il.Emit(OpCodes.Initobj, ret);
                il.Emit(OpCodes.Ldloc, v);
            }
            else il.Emit(OpCodes.Ldnull);
        }
        il.Emit(OpCodes.Ret);
    }
}
