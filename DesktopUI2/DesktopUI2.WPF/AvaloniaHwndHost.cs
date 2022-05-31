﻿using Avalonia.Controls.Embedding;
using Avalonia.Input;
using Avalonia.Platform;

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DesktopUI2.WPF
{
  public class AvaloniaHwndHost : HwndHost
  {
    public static IntPtr hWndAlternativeParent;
    private readonly EmbeddableControlRoot _root = new EmbeddableControlRoot();

    public AvaloniaHwndHost()
    {
      DataContextChanged += AvaloniaHwndHost_DataContextChanged;
    }

    private void AvaloniaHwndHost_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
      if (Content != null)
      {
        Content.DataContext = e.NewValue;
      }
    }

    public Avalonia.Controls.Control Content
    {
      get => (Avalonia.Controls.Control)_root.Content;
      set
      {
        _root.Content = value;
        if (value != null)
        {
          value.DataContext = DataContext;
        }
      }
    }

    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
      _root.Prepare();
      _root.Renderer.Start();

      var z = IntPtr.Zero;

      var z2 = ((IWindowImpl)_root.PlatformImpl)?.Handle?.Handle;

      var handle = ((IWindowImpl)_root.PlatformImpl)?.Handle?.Handle ?? IntPtr.Zero;

      var wpfWindow = Window.GetWindow(this);


      var parentHandle = (wpfWindow == null) ? hWndAlternativeParent : new WindowInteropHelper(wpfWindow).Handle;
      _ = UnmanagedMethods.SetParent(handle, parentHandle);

      if (IsFocused)
      {
        // Focus Avalonia, if host was focused before handle was created.
        FocusManager.Instance.Focus(null);
      }

      return new HandleRef(_root, handle);
    }

    protected override void DestroyWindowCore(HandleRef hwnd)
    {
      _root.Dispose();
    }
  }


  internal unsafe static class UnmanagedMethods
  {
    [DllImport("user32.dll")]
    public static extern bool SetParent(IntPtr hWnd, IntPtr hWndNewParent);
  }
}
