using System;
using SandBox.View.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace CheatMenu.UI
{
    /// <summary>
    /// Bir ekrana (varsayılan: harita, MapScreen) eklenen Gauntlet katmanı ve içindeki tek "movie" (prefab + ViewModel).
    /// Haritadaysa: harita ekranı kapanır ya da yenisi açılırsa (kayıt yükleme) katman ölü sayılır. Başka bir ekrandaysa
    /// (Klan ekranındaki Entrika sekmesinden açılan olay kartı): o ekran kapanınca.
    /// </summary>
    public sealed class MovieLayer
    {
        private GauntletLayer _layer;
        private GauntletMovieIdentifier _movie;
        private ScreenBase _screen;
        private bool _onMap;
        private bool _focused;

        public ViewModel DataSource { get; private set; }

        public bool IsOpen => _layer != null;

        /// <summary>Açık ve bağlı olduğu ekran hâlâ duruyor mu (haritadaysa: şu anki harita ekranı mı).</summary>
        public bool IsAlive => _layer != null && _screen != null && !_layer.IsFinalized
            && (_onMap ? _screen == MapScreen.Instance : !_screen.IsFinalized);

        /// <param name="modal">true: alttaki her şeyi (harita, tuşlar) engeller ve odağı alır. false: yalnızca üstüne tıklanınca fare olayını alır.</param>
        /// <param name="host">null: harita ekranı.</param>
        public bool Open(string movieName, ViewModel dataSource, int order, bool modal, ScreenBase host = null)
        {
            Close();
            ScreenBase screen = host ?? MapScreen.Instance;
            if (screen == null) return false;
            var layer = new GauntletLayer("TacVeHancer_" + movieName, order);
            bool added = false;
            try
            {
                _movie = layer.LoadMovie(movieName, dataSource);
                layer.InputRestrictions.SetInputRestrictions(true, modal ? InputUsageMask.All : InputUsageMask.Mouse);
                screen.AddLayer(layer);
                added = true;
                if (modal)
                {
                    layer.IsFocusLayer = true;
                    ScreenManager.TrySetFocus(layer);
                    _focused = true;
                }
            }
            catch
            {
                // Yarım kalan katman haritada kalıp girdiyi engellemesin.
                try
                {
                    if (_focused) ScreenManager.TryLoseFocus(layer);
                    layer.InputRestrictions.ResetInputRestrictions();
                    if (_movie != null) layer.ReleaseMovie(_movie);
                    if (added) screen.RemoveLayer(layer);
                }
                catch (Exception cleanup)
                {
                    Debug.Print("[CheatMenu] layer cleanup failed: " + cleanup);
                }
                _movie = null;
                _focused = false;
                throw;
            }
            _layer = layer;
            _screen = screen;
            _onMap = host == null;
            DataSource = dataSource;
            return true;
        }

        /// <summary>Üstteki bir pencere kapandıktan sonra klavye odağını yeniden bu katmana verir (yalnızca modal katmanlar).</summary>
        public void Refocus()
        {
            if (!_focused || !IsAlive) return;
            _layer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_layer);
        }

        public void Close()
        {
            GauntletLayer layer = _layer;
            ScreenBase screen = _screen;
            ViewModel dataSource = DataSource;
            _layer = null;
            _screen = null;
            DataSource = null;
            if (layer != null)
            {
                try
                {
                    if (_focused)
                    {
                        layer.IsFocusLayer = false;
                        ScreenManager.TryLoseFocus(layer);
                    }
                    layer.InputRestrictions.ResetInputRestrictions();
                    if (_movie != null && !layer.IsFinalized) layer.ReleaseMovie(_movie);
                    if (screen != null && !layer.IsFinalized) screen.RemoveLayer(layer);
                }
                catch (Exception ex)
                {
                    Debug.Print("[CheatMenu] layer close failed: " + ex);
                }
            }
            _movie = null;
            _focused = false;
            try
            {
                dataSource?.OnFinalize();
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] view model finalize failed: " + ex);
            }
        }
    }
}
