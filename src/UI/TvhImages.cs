using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.TwoDimension;
using NativeTexture = TaleWorlds.Engine.Texture;
using UITexture = TaleWorlds.TwoDimension.Texture;

namespace CheatMenu.UI
{
    /// <summary>
    /// Modülün Images klasöründeki resimleri (olay kartları, ekran arka planı) çalışma anında yükler.
    /// Oyunun sprite paketine çevirme gerekmez: PNG/JPG klasöre konur, ad ile çağrılır. Dosya yoksa null döner;
    /// arayüz o zaman resimsiz düzene geçer. Bellek için yalnızca son birkaç resim önbellekte tutulur.
    /// </summary>
    public static class TvhImages
    {
        private const int CacheSize = 8;
        private static readonly string[] Extensions = { ".png", ".jpg", ".jpeg" };
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();
        private static readonly LinkedList<string> Order = new LinkedList<string>();
        private static readonly HashSet<string> Failed = new HashSet<string>();

        public static string Folder => Path.Combine(ModuleHelper.GetModuleFullPath(ModInfo.Id), "Images");

        /// <summary>Resim dosyası var mı (yüklemeden bakar).</summary>
        public static bool Exists(string name) => !string.IsNullOrEmpty(name) && FindFile(name) != null;

        /// <summary>Adı verilen resmin sprite'ı; dosya yoksa ya da açılamazsa null.</summary>
        public static Sprite Get(string name)
        {
            if (string.IsNullOrEmpty(name) || Failed.Contains(name)) return null;
            if (Cache.TryGetValue(name, out Sprite cached))
            {
                Order.Remove(name);
                Order.AddFirst(name);
                return cached;
            }
            string file = FindFile(name);
            if (file == null) return null;
            try
            {
                NativeTexture texture = Load(file) ?? throw new InvalidOperationException("doku oluşturulamadı");
                var sprite = new ImageSprite(name, new UITexture(new EngineTexture(texture)));
                Cache[name] = sprite;
                Order.AddFirst(name);
                while (Order.Count > CacheSize)
                {
                    Cache.Remove(Order.Last.Value);
                    Order.RemoveLast();
                }
                return sprite;
            }
            catch (Exception ex)
            {
                Failed.Add(name);
                Debug.Print("[CheatMenu] image " + file + " could not be loaded: " + ex);
                return null;
            }
        }

        /// <summary>Önce dosyayı bellekten çözer; olmazsa motorun dosyadan yükleyicisini dener.</summary>
        private static NativeTexture Load(string file)
        {
            try
            {
                NativeTexture texture = NativeTexture.CreateFromMemory(File.ReadAllBytes(file));
                if (texture != null) return texture;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] CreateFromMemory failed for " + file + ": " + ex.Message);
            }
            return NativeTexture.LoadTextureFromPath(Path.GetFileName(file), Path.GetDirectoryName(file) + Path.DirectorySeparatorChar);
        }

        private static string FindFile(string name)
        {
            try
            {
                foreach (string extension in Extensions)
                {
                    string file = Path.Combine(Folder, name + extension);
                    if (File.Exists(file)) return file;
                }
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] image folder lookup failed: " + ex);
            }
            return null;
        }

        /// <summary>Bütün dokuyu kaplayan basit sprite (atlas yok, dokuz parça yok).</summary>
        private sealed class ImageSprite : Sprite
        {
            private readonly UITexture _texture;

            public ImageSprite(string name, UITexture texture)
                : base("tvh_" + name, texture.Width, texture.Height, SpriteNinePatchParameters.Empty)
            {
                _texture = texture;
            }

            public override UITexture Texture => _texture;
            public override Vec2 GetMinUvs() => new Vec2(0f, 0f);
            public override Vec2 GetMaxUvs() => new Vec2(1f, 1f);
        }
    }
}
