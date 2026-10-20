using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

// Regard and Standing, isteğe bağlı Calradia Remembers'la konuşan bir sınıf taşıyor (PairRegardSource+Source, o modun
// IPairRegardSource arayüzünü uyguluyor). Calradia Remembers kurulu değilken oyun bu sınıfı yükleyemiyor ve her açılışta
// "submodule could not be loaded correctly due to a dependency conflict" penceresini gösteriyor. Bu araç arayüzü söker ve
// sınıfı kaydeden tek metodu (RegisterNow) boşaltır; modun geri kalanına dokunmaz. Calradia Remembers'ın diğer bağlantıları
// yalnızca metot gövdelerinde ve mod onları zaten Calradia Remembers yoksa çağırmıyor.
// Kullanım: RsPatch <RegardStanding.dll>  (yerinde değiştirir; ikinci kez çalıştırmak bir şey yapmaz)
static class Program
{
    static int Main(string[] args)
    {
        if (args.Length != 1 || !File.Exists(args[0])) { Console.WriteLine("Kullanım: RsPatch <RegardStanding.dll>"); return 2; }
        string path = args[0];
        var module = ModuleDefinition.ReadModule(new MemoryStream(File.ReadAllBytes(path)));
        TypeDefinition owner = module.GetType("RegardStanding.PairRegardSource");
        TypeDefinition source = owner?.NestedTypes.FirstOrDefault(t => t.Name == "Source");
        MethodDefinition register = owner?.Methods.FirstOrDefault(m => m.Name == "RegisterNow");
        if (source == null || register == null) { Console.WriteLine("Beklenen sınıf bulunamadı: Regard and Standing'in sürümü değişmiş, araç güncellenmeli"); return 1; }

        bool changed = false;
        foreach (InterfaceImplementation i in source.Interfaces.ToList())
            if (i.InterfaceType.Scope.Name.StartsWith("CalradiaRemembers", StringComparison.Ordinal))
            {
                source.Interfaces.Remove(i);
                changed = true;
            }
        if (register.Body.Instructions.Count > 1)
        {
            register.Body.Instructions.Clear();
            register.Body.Variables.Clear();
            register.Body.ExceptionHandlers.Clear();
            register.Body.GetILProcessor().Emit(OpCodes.Ret);
            changed = true;
        }
        // Arayüz metodunun "arayüzü uygular" işaretleri kalmasın (açık uygulama yoksa yalnızca virtual/final bayrakları).
        foreach (MethodDefinition m in source.Methods) m.Overrides.Clear();

        if (!changed) { Console.WriteLine("Zaten yamalı: " + path); return 0; }
        module.Write(path);
        Console.WriteLine("Yamandı: " + path);
        return 0;
    }
}
