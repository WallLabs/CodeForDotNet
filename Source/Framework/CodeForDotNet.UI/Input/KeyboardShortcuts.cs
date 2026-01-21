using System;

namespace CodeForDotNet.UI.Input;

/// <summary>
/// Keyboard shortcut data, disconnected from any API dependencies, useful for future proof serialization and cross-platform. A copy of
/// System.Windows.Forms.Shortcut. Specifies shortcut keys that can be used by menu items.
/// </summary>
[Flags]
public enum KeyboardShortcuts
{
    /// <summary>
    /// No shortcut key is associated with the menu item.
    /// </summary>
    None = 0,

    /// <summary>
    /// The shortcut key INSERT.
    /// </summary>
    Ins = 45,

    /// <summary>
    /// The shortcut key DELETE.
    /// </summary>
    Del = 46,

    /// <summary>
    /// The shortcut key F1.
    /// </summary>
    F1 = 112,

    /// <summary>
    /// The shortcut key F2.
    /// </summary>
    F2 = 113,

    /// <summary>
    /// The shortcut key F3.
    /// </summary>
    F3 = 114,

    /// <summary>
    /// The shortcut key F4.
    /// </summary>
    F4 = 115,

    /// <summary>
    /// The shortcut key F5.
    /// </summary>
    F5 = 116,

    /// <summary>
    /// The shortcut key F6.
    /// </summary>
    F6 = 117,

    /// <summary>
    /// The shortcut key F7.
    /// </summary>
    F7 = 118,

    /// <summary>
    /// The shortcut key F8.
    /// </summary>
    F8 = 119,

    /// <summary>
    /// The shortcut key F9.
    /// </summary>
    F9 = 120,

    /// <summary>
    /// The shortcut key F10.
    /// </summary>
    F10 = 121,

    /// <summary>
    /// The shortcut key F11.
    /// </summary>
    F11 = 122,

    /// <summary>
    /// The shortcut key F12.
    /// </summary>
    F12 = 123,

    /// <summary>
    /// The shortcut keys SHIFT+INSERT.
    /// </summary>
    ShiftIns = 65581,

    /// <summary>
    /// The shortcut keys SHIFT+DELETE.
    /// </summary>
    ShiftDel = 65582,

    /// <summary>
    /// The shortcut keys SHIFT+F1.
    /// </summary>
    ShiftF1 = 65648,

    /// <summary>
    /// The shortcut keys SHIFT+F2.
    /// </summary>
    ShiftF2 = 65649,

    /// <summary>
    /// The shortcut keys SHIFT+F3.
    /// </summary>
    ShiftF3 = 65650,

    /// <summary>
    /// The shortcut keys SHIFT+F4.
    /// </summary>
    ShiftF4 = 65651,

    /// <summary>
    /// The shortcut keys SHIFT+F5.
    /// </summary>
    ShiftF5 = 65652,

    /// <summary>
    /// The shortcut keys SHIFT+F6.
    /// </summary>
    ShiftF6 = 65653,

    /// <summary>
    /// The shortcut keys SHIFT+F7.
    /// </summary>
    ShiftF7 = 65654,

    /// <summary>
    /// The shortcut keys SHIFT+F8.
    /// </summary>
    ShiftF8 = 65655,

    /// <summary>
    /// The shortcut keys SHIFT+F9.
    /// </summary>
    ShiftF9 = 65656,

    /// <summary>
    /// The shortcut keys SHIFT+F10.
    /// </summary>
    ShiftF10 = 65657,

    /// <summary>
    /// The shortcut keys SHIFT+F11.
    /// </summary>
    ShiftF11 = 65658,

    /// <summary>
    /// The shortcut keys SHIFT+F12.
    /// </summary>
    ShiftF12 = 65659,

    /// <summary>
    /// The shortcut keys CTRL+INSERT.
    /// </summary>
    CtrlIns = 131117,

    /// <summary>
    /// The shortcut keys CTRL+DELETE.
    /// </summary>
    CtrlDel = 131118,

    /// <summary>
    /// The shortcut keys CTRL+0.
    /// </summary>
    Ctrl0 = 131120,

    /// <summary>
    /// The shortcut keys CTRL+1.
    /// </summary>
    Ctrl1 = 131121,

    /// <summary>
    /// The shortcut keys CTRL+2.
    /// </summary>
    Ctrl2 = 131122,

    /// <summary>
    /// The shortcut keys CTRL+3.
    /// </summary>
    Ctrl3 = 131123,

    /// <summary>
    /// The shortcut keys CTRL+4.
    /// </summary>
    Ctrl4 = 131124,

    /// <summary>
    /// The shortcut keys CTRL+5.
    /// </summary>
    Ctrl5 = 131125,

    /// <summary>
    /// The shortcut keys CTRL+6.
    /// </summary>
    Ctrl6 = 131126,

    /// <summary>
    /// The shortcut keys CTRL+7.
    /// </summary>
    Ctrl7 = 131127,

    /// <summary>
    /// The shortcut keys CTRL+8.
    /// </summary>
    Ctrl8 = 131128,

    /// <summary>
    /// The shortcut keys CTRL+9.
    /// </summary>
    Ctrl9 = 131129,

    /// <summary>
    /// The shortcut keys CTRL+A.
    /// </summary>
    CtrlA = 131137,

    /// <summary>
    /// The shortcut keys CTRL+B.
    /// </summary>
    CtrlB = 131138,

    /// <summary>
    /// The shortcut keys CTRL+C.
    /// </summary>
    CtrlC = 131139,

    /// <summary>
    /// The shortcut keys CTRL+D.
    /// </summary>
    CtrlD = 131140,

    /// <summary>
    /// The shortcut keys CTRL+E.
    /// </summary>
    CtrlE = 131141,

    /// <summary>
    /// The shortcut keys CTRL+F.
    /// </summary>
    CtrlF = 131142,

    /// <summary>
    /// The shortcut keys CTRL+G.
    /// </summary>
    CtrlG = 131143,

    /// <summary>
    /// The shortcut keys CTRL+H.
    /// </summary>
    CtrlH = 131144,

    /// <summary>
    /// The shortcut keys CTRL+I.
    /// </summary>
    CtrlI = 131145,

    /// <summary>
    /// The shortcut keys CTRL+J.
    /// </summary>
    CtrlJ = 131146,

    /// <summary>
    /// The shortcut keys CTRL+K.
    /// </summary>
    CtrlK = 131147,

    /// <summary>
    /// The shortcut keys CTRL+L.
    /// </summary>
    CtrlL = 131148,

    /// <summary>
    /// The shortcut keys CTRL+M.
    /// </summary>
    CtrlM = 131149,

    /// <summary>
    /// The shortcut keys CTRL+N.
    /// </summary>
    CtrlN = 131150,

    /// <summary>
    /// The shortcut keys CTRL+O.
    /// </summary>
    CtrlO = 131151,

    /// <summary>
    /// The shortcut keys CTRL+P.
    /// </summary>
    CtrlP = 131152,

    /// <summary>
    /// The shortcut keys CTRL+Q.
    /// </summary>
    CtrlQ = 131153,

    /// <summary>
    /// The shortcut keys CTRL+R.
    /// </summary>
    CtrlR = 131154,

    /// <summary>
    /// The shortcut keys CTRL+S.
    /// </summary>
    CtrlS = 131155,

    /// <summary>
    /// The shortcut keys CTRL+T.
    /// </summary>
    CtrlT = 131156,

    /// <summary>
    /// The shortcut keys CTRL+U.
    /// </summary>
    CtrlU = 131157,

    /// <summary>
    /// The shortcut keys CTRL+V.
    /// </summary>
    CtrlV = 131158,

    /// <summary>
    /// The shortcut keys CTRL+W.
    /// </summary>
    CtrlW = 131159,

    /// <summary>
    /// The shortcut keys CTRL+X.
    /// </summary>
    CtrlX = 131160,

    /// <summary>
    /// The shortcut keys CTRL+Y.
    /// </summary>
    CtrlY = 131161,

    /// <summary>
    /// The shortcut keys CTRL+Z.
    /// </summary>
    CtrlZ = 131162,

    /// <summary>
    /// The shortcut keys CTRL+F1.
    /// </summary>
    CtrlF1 = 131184,

    /// <summary>
    /// The shortcut keys CTRL+F2.
    /// </summary>
    CtrlF2 = 131185,

    /// <summary>
    /// The shortcut keys CTRL+F3.
    /// </summary>
    CtrlF3 = 131186,

    /// <summary>
    /// The shortcut keys CTRL+F4.
    /// </summary>
    CtrlF4 = 131187,

    /// <summary>
    /// The shortcut keys CTRL+F5.
    /// </summary>
    CtrlF5 = 131188,

    /// <summary>
    /// The shortcut keys CTRL+F6.
    /// </summary>
    CtrlF6 = 131189,

    /// <summary>
    /// The shortcut keys CTRL+F7.
    /// </summary>
    CtrlF7 = 131190,

    /// <summary>
    /// The shortcut keys CTRL+F8.
    /// </summary>
    CtrlF8 = 131191,

    /// <summary>
    /// The shortcut keys CTRL+F9.
    /// </summary>
    CtrlF9 = 131192,

    /// <summary>
    /// The shortcut keys CTRL+F10.
    /// </summary>
    CtrlF10 = 131193,

    /// <summary>
    /// The shortcut keys CTRL+F11.
    /// </summary>
    CtrlF11 = 131194,

    /// <summary>
    /// The shortcut keys CTRL+F12.
    /// </summary>
    CtrlF12 = 131195,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+0.
    /// </summary>
    CtrlShift0 = 196656,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+1.
    /// </summary>
    CtrlShift1 = 196657,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+2.
    /// </summary>
    CtrlShift2 = 196658,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+3.
    /// </summary>
    CtrlShift3 = 196659,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+4.
    /// </summary>
    CtrlShift4 = 196660,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+5.
    /// </summary>
    CtrlShift5 = 196661,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+6.
    /// </summary>
    CtrlShift6 = 196662,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+7.
    /// </summary>
    CtrlShift7 = 196663,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+8.
    /// </summary>
    CtrlShift8 = 196664,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+9.
    /// </summary>
    CtrlShift9 = 196665,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+A.
    /// </summary>
    CtrlShiftA = 196673,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+B.
    /// </summary>
    CtrlShiftB = 196674,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+C.
    /// </summary>
    CtrlShiftC = 196675,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+D.
    /// </summary>
    CtrlShiftD = 196676,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+E.
    /// </summary>
    CtrlShiftE = 196677,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F.
    /// </summary>
    CtrlShiftF = 196678,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+G.
    /// </summary>
    CtrlShiftG = 196679,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+H.
    /// </summary>
    CtrlShiftH = 196680,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+I.
    /// </summary>
    CtrlShiftI = 196681,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+J.
    /// </summary>
    CtrlShiftJ = 196682,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+K.
    /// </summary>
    CtrlShiftK = 196683,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+L.
    /// </summary>
    CtrlShiftL = 196684,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+M.
    /// </summary>
    CtrlShiftM = 196685,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+N.
    /// </summary>
    CtrlShiftN = 196686,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+O.
    /// </summary>
    CtrlShiftO = 196687,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+P.
    /// </summary>
    CtrlShiftP = 196688,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+Q.
    /// </summary>
    CtrlShiftQ = 196689,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+R.
    /// </summary>
    CtrlShiftR = 196690,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+S.
    /// </summary>
    CtrlShiftS = 196691,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+T.
    /// </summary>
    CtrlShiftT = 196692,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+U.
    /// </summary>
    CtrlShiftU = 196693,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+V.
    /// </summary>
    CtrlShiftV = 196694,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+W.
    /// </summary>
    CtrlShiftW = 196695,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+X.
    /// </summary>
    CtrlShiftX = 196696,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+Y.
    /// </summary>
    CtrlShiftY = 196697,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+Z.
    /// </summary>
    CtrlShiftZ = 196698,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F1.
    /// </summary>
    CtrlShiftF1 = 196720,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F2.
    /// </summary>
    CtrlShiftF2 = 196721,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F3.
    /// </summary>
    CtrlShiftF3 = 196722,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F4.
    /// </summary>
    CtrlShiftF4 = 196723,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F5.
    /// </summary>
    CtrlShiftF5 = 196724,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F6.
    /// </summary>
    CtrlShiftF6 = 196725,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F7.
    /// </summary>
    CtrlShiftF7 = 196726,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F8.
    /// </summary>
    CtrlShiftF8 = 196727,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F9.
    /// </summary>
    CtrlShiftF9 = 196728,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F10.
    /// </summary>
    CtrlShiftF10 = 196729,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F11.
    /// </summary>
    CtrlShiftF11 = 196730,

    /// <summary>
    /// The shortcut keys CTRL+SHIFT+F12.
    /// </summary>
    CtrlShiftF12 = 196731,

    /// <summary>
    /// The shortcut keys ALT+BACKSPACE.
    /// </summary>
    AltBksp = 262152,

    /// <summary>
    /// The shortcut keys ALT+LEFTARROW.
    /// </summary>
    AltLeftArrow = 262181,

    /// <summary>
    /// The shortcut keys ALT+UPARROW.
    /// </summary>
    AltUpArrow = 262182,

    /// <summary>
    /// The shortcut keys ALT+RIGHTARROW.
    /// </summary>
    AltRightArrow = 262183,

    /// <summary>
    /// The shortcut keys ALT+DOWNARROW.
    /// </summary>
    AltDownArrow = 262184,

    /// <summary>
    /// The shortcut keys ALT+0.
    /// </summary>
    Alt0 = 262192,

    /// <summary>
    /// The shortcut keys ALT+1.
    /// </summary>
    Alt1 = 262193,

    /// <summary>
    /// The shortcut keys ALT+2.
    /// </summary>
    Alt2 = 262194,

    /// <summary>
    /// The shortcut keys ALT+3.
    /// </summary>
    Alt3 = 262195,

    /// <summary>
    /// The shortcut keys ALT+4.
    /// </summary>
    Alt4 = 262196,

    /// <summary>
    /// The shortcut keys ALT+5.
    /// </summary>
    Alt5 = 262197,

    /// <summary>
    /// The shortcut keys ALT+6.
    /// </summary>
    Alt6 = 262198,

    /// <summary>
    /// The shortcut keys ALT+7.
    /// </summary>
    Alt7 = 262199,

    /// <summary>
    /// The shortcut keys ALT+8.
    /// </summary>
    Alt8 = 262200,

    /// <summary>
    /// The shortcut keys ALT+9.
    /// </summary>
    Alt9 = 262201,

    /// <summary>
    /// The shortcut keys ALT+F1.
    /// </summary>
    AltF1 = 262256,

    /// <summary>
    /// The shortcut keys ALT+F2.
    /// </summary>
    AltF2 = 262257,

    /// <summary>
    /// The shortcut keys ALT+F3.
    /// </summary>
    AltF3 = 262258,

    /// <summary>
    /// The shortcut keys ALT+F4.
    /// </summary>
    AltF4 = 262259,

    /// <summary>
    /// The shortcut keys ALT+F5.
    /// </summary>
    AltF5 = 262260,

    /// <summary>
    /// The shortcut keys ALT+F6.
    /// </summary>
    AltF6 = 262261,

    /// <summary>
    /// The shortcut keys ALT+F7.
    /// </summary>
    AltF7 = 262262,

    /// <summary>
    /// The shortcut keys ALT+F8.
    /// </summary>
    AltF8 = 262263,

    /// <summary>
    /// The shortcut keys ALT+F9.
    /// </summary>
    AltF9 = 262264,

    /// <summary>
    /// The shortcut keys ALT+F10.
    /// </summary>
    AltF10 = 262265,

    /// <summary>
    /// The shortcut keys ALT+F11.
    /// </summary>
    AltF11 = 262266,

    /// <summary>
    /// The shortcut keys ALT+F12.
    /// </summary>
    AltF12 = 262267
}
