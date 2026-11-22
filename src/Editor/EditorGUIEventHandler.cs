using System;
using System.Runtime.CompilerServices;
using EniGUI.LowLevel;
using UnityEngine;

namespace EniGUI.Editor
{
    internal class EditorGUIEventHandler
    {
        private const int KEY_CODE_COUNT = 680;
        private static GUIKeyCode[] KeyMap;

        private GUIInputTracker.Input m_CurrentInput = new();
        
        public GUIHitTracker.Hit CurrentHit = new();

        public void Handle(Event e)
        {
            bool hitUpdateRequirement = false;

            switch(e.type)
            {
                case EventType.MouseDown:
                {
                    var key = MouseKey(e.button);
                    if (!m_CurrentInput.Held.Has(key))
                        m_CurrentInput.Pressed.Set(key);
                    m_CurrentInput.Held.Set(key);
                    SetPointer(e.mousePosition);
                    hitUpdateRequirement = true;
                    break;
                }
                case EventType.MouseUp:
                {
                    var key = MouseKey(e.button);
                    m_CurrentInput.Released.Set(key);
                    m_CurrentInput.Held.Remove(key);
                    SetPointer(e.mousePosition);
                    hitUpdateRequirement = true;
                    break;
                }
                case EventType.MouseMove:
                case EventType.MouseDrag:
                {
                    SetPointer(e.mousePosition);
                    hitUpdateRequirement = true;
                    break;
                }
                case EventType.ScrollWheel:
                {
                    m_CurrentInput.Scroll += e.delta;
                    hitUpdateRequirement = true;
                    break;
                }
                case EventType.KeyDown:
                {
                    var key = ConvertKey(e.keyCode);
                    if (key == GUIKeyCode.None)
                        break;

                    if (!m_CurrentInput.Held.Has(key))
                        m_CurrentInput.Pressed.Set(key); 
                        
                    m_CurrentInput.Held.Set(key);
                    break;
                }
                case EventType.KeyUp:
                {
                    var key = ConvertKey(e.keyCode);
                    if (key == GUIKeyCode.None)
                        break;

                    m_CurrentInput.Released.Set(key);
                    m_CurrentInput.Held.Remove(key);
                    break;
                }
                case EventType.MouseLeaveWindow:
                {
                    m_CurrentInput.Held.Clear();
                    hitUpdateRequirement = true;
                    break;
                }
                case EventType.MouseEnterWindow:
                {
                    hitUpdateRequirement = true;
                    break;
                }
            }

            EventModifiers modifiers = e.modifiers;

            SyncModifier(modifiers, EventModifiers.Control, GUIKeyCode.LeftControl, GUIKeyCode.RightControl);
            SyncModifier(modifiers, EventModifiers.Shift, GUIKeyCode.LeftShift, GUIKeyCode.RightShift);
            SyncModifier(modifiers, EventModifiers.Alt, GUIKeyCode.LeftAlt, GUIKeyCode.RightAlt);
            SyncModifier(modifiers, EventModifiers.Command, GUIKeyCode.LeftCommand, GUIKeyCode.RightCommand);

            if(hitUpdateRequirement)
            {
                if(e.type != EventType.MouseLeaveWindow)
                {
                    var id = GUILowLevel.PickID(e.mousePosition);

                    CurrentHit.Hover = id;

                    if (e.type == EventType.MouseDown)
                        CurrentHit.Active = id;
                    else if(e.type == EventType.MouseUp)
                        CurrentHit.Active = 0;
                }
                else
                {
                    CurrentHit.Hover = 0;
                    CurrentHit.Active = 0;
                }
            }
        }

        public GUIInputTracker.Input Capture()
        {
            var result = m_CurrentInput;

            m_CurrentInput.Pressed.Clear();
            m_CurrentInput.Released.Clear();
            m_CurrentInput.PointerDelta = default;
            m_CurrentInput.Scroll = default;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SyncModifier(EventModifiers modifiers, EventModifiers flag, 
            GUIKeyCode leftKey, GUIKeyCode rightKey)
        {
            if((modifiers & flag) == 0)
            {
                m_CurrentInput.Held.Remove(leftKey);
                m_CurrentInput.Held.Remove(rightKey);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GUIKeyCode MouseKey(int button)
        {
            return button switch
            {
                0 => GUIKeyCode.Mouse0,
                1 => GUIKeyCode.Mouse1,
                2 => GUIKeyCode.Mouse2,
                3 => GUIKeyCode.Mouse3,
                4 => GUIKeyCode.Mouse4,
                5 => GUIKeyCode.Mouse5,
                6 => GUIKeyCode.Mouse6,
                _ => throw new ArgumentException(""),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetPointer(Vector2 position)
        {
            m_CurrentInput.PointerDelta += position - m_CurrentInput.Pointer;
            m_CurrentInput.Pointer = position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GUIKeyCode ConvertKey(KeyCode key)
        {
            if(KeyMap == null)
                KeyMap = BuildKeyMap();

            return KeyMap[(int)key];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static GUIKeyCode[] BuildKeyMap()
        {
            
            GUIKeyCode[] map = new GUIKeyCode[KEY_CODE_COUNT];

            // --- None ---
            map[(int)KeyCode.None] = GUIKeyCode.None;

            // --- ����������� / ��������� ---
            map[(int)KeyCode.Backspace] = GUIKeyCode.Backspace;
            map[(int)KeyCode.Tab] = GUIKeyCode.Tab;
            map[(int)KeyCode.Clear] = GUIKeyCode.Clear;
            map[(int)KeyCode.Return] = GUIKeyCode.Return;
            map[(int)KeyCode.Pause] = GUIKeyCode.Pause;
            map[(int)KeyCode.Escape] = GUIKeyCode.Escape;
            map[(int)KeyCode.Space] = GUIKeyCode.Space;

            // --- ������� ---
            map[(int)KeyCode.Exclaim] = GUIKeyCode.Exclaim;
            map[(int)KeyCode.DoubleQuote] = GUIKeyCode.DoubleQuote;
            map[(int)KeyCode.Hash] = GUIKeyCode.Hash;
            map[(int)KeyCode.Dollar] = GUIKeyCode.Dollar;
            map[(int)KeyCode.Percent] = GUIKeyCode.Percent;
            map[(int)KeyCode.Ampersand] = GUIKeyCode.Ampersand;
            map[(int)KeyCode.Quote] = GUIKeyCode.Quote;
            map[(int)KeyCode.LeftParen] = GUIKeyCode.LeftParen;
            map[(int)KeyCode.RightParen] = GUIKeyCode.RightParen;
            map[(int)KeyCode.Asterisk] = GUIKeyCode.Asterisk;
            map[(int)KeyCode.Plus] = GUIKeyCode.Plus;
            map[(int)KeyCode.Comma] = GUIKeyCode.Comma;
            map[(int)KeyCode.Minus] = GUIKeyCode.Minus;
            map[(int)KeyCode.Period] = GUIKeyCode.Period;
            map[(int)KeyCode.Slash] = GUIKeyCode.Slash;
            map[(int)KeyCode.Colon] = GUIKeyCode.Colon;
            map[(int)KeyCode.Semicolon] = GUIKeyCode.Semicolon;
            map[(int)KeyCode.Less] = GUIKeyCode.Less;
            map[(int)KeyCode.Equals] = GUIKeyCode.Equals;
            map[(int)KeyCode.Greater] = GUIKeyCode.Greater;
            map[(int)KeyCode.Question] = GUIKeyCode.Question;
            map[(int)KeyCode.At] = GUIKeyCode.At;
            map[(int)KeyCode.LeftBracket] = GUIKeyCode.LeftBracket;
            map[(int)KeyCode.Backslash] = GUIKeyCode.Backslash;
            map[(int)KeyCode.RightBracket] = GUIKeyCode.RightBracket;
            map[(int)KeyCode.Caret] = GUIKeyCode.Caret;
            map[(int)KeyCode.Underscore] = GUIKeyCode.Underscore;
            map[(int)KeyCode.BackQuote] = GUIKeyCode.BackQuote;
            map[(int)KeyCode.LeftCurlyBracket] = GUIKeyCode.LeftCurlyBracket;
            map[(int)KeyCode.Pipe] = GUIKeyCode.Pipe;
            map[(int)KeyCode.RightCurlyBracket] = GUIKeyCode.RightCurlyBracket;
            map[(int)KeyCode.Tilde] = GUIKeyCode.Tilde;

            // --- ����� �������� ���� ---
            map[(int)KeyCode.Alpha0] = GUIKeyCode.Alpha0;
            map[(int)KeyCode.Alpha1] = GUIKeyCode.Alpha1;
            map[(int)KeyCode.Alpha2] = GUIKeyCode.Alpha2;
            map[(int)KeyCode.Alpha3] = GUIKeyCode.Alpha3;
            map[(int)KeyCode.Alpha4] = GUIKeyCode.Alpha4;
            map[(int)KeyCode.Alpha5] = GUIKeyCode.Alpha5;
            map[(int)KeyCode.Alpha6] = GUIKeyCode.Alpha6;
            map[(int)KeyCode.Alpha7] = GUIKeyCode.Alpha7;
            map[(int)KeyCode.Alpha8] = GUIKeyCode.Alpha8;
            map[(int)KeyCode.Alpha9] = GUIKeyCode.Alpha9;

            // --- ����� ---
            map[(int)KeyCode.A] = GUIKeyCode.A;
            map[(int)KeyCode.B] = GUIKeyCode.B;
            map[(int)KeyCode.C] = GUIKeyCode.C;
            map[(int)KeyCode.D] = GUIKeyCode.D;
            map[(int)KeyCode.E] = GUIKeyCode.E;
            map[(int)KeyCode.F] = GUIKeyCode.F;
            map[(int)KeyCode.G] = GUIKeyCode.G;
            map[(int)KeyCode.H] = GUIKeyCode.H;
            map[(int)KeyCode.I] = GUIKeyCode.I;
            map[(int)KeyCode.J] = GUIKeyCode.J;
            map[(int)KeyCode.K] = GUIKeyCode.K;
            map[(int)KeyCode.L] = GUIKeyCode.L;
            map[(int)KeyCode.M] = GUIKeyCode.M;
            map[(int)KeyCode.N] = GUIKeyCode.N;
            map[(int)KeyCode.O] = GUIKeyCode.O;
            map[(int)KeyCode.P] = GUIKeyCode.P;
            map[(int)KeyCode.Q] = GUIKeyCode.Q;
            map[(int)KeyCode.R] = GUIKeyCode.R;
            map[(int)KeyCode.S] = GUIKeyCode.S;
            map[(int)KeyCode.T] = GUIKeyCode.T;
            map[(int)KeyCode.U] = GUIKeyCode.U;
            map[(int)KeyCode.V] = GUIKeyCode.V;
            map[(int)KeyCode.W] = GUIKeyCode.W;
            map[(int)KeyCode.X] = GUIKeyCode.X;
            map[(int)KeyCode.Y] = GUIKeyCode.Y;
            map[(int)KeyCode.Z] = GUIKeyCode.Z;

            map[(int)KeyCode.Delete] = GUIKeyCode.Delete;

            // --- Numpad ---
            map[(int)KeyCode.Keypad0] = GUIKeyCode.Keypad0;
            map[(int)KeyCode.Keypad1] = GUIKeyCode.Keypad1;
            map[(int)KeyCode.Keypad2] = GUIKeyCode.Keypad2;
            map[(int)KeyCode.Keypad3] = GUIKeyCode.Keypad3;
            map[(int)KeyCode.Keypad4] = GUIKeyCode.Keypad4;
            map[(int)KeyCode.Keypad5] = GUIKeyCode.Keypad5;
            map[(int)KeyCode.Keypad6] = GUIKeyCode.Keypad6;
            map[(int)KeyCode.Keypad7] = GUIKeyCode.Keypad7;
            map[(int)KeyCode.Keypad8] = GUIKeyCode.Keypad8;
            map[(int)KeyCode.Keypad9] = GUIKeyCode.Keypad9;
            map[(int)KeyCode.KeypadPeriod] = GUIKeyCode.KeypadPeriod;
            map[(int)KeyCode.KeypadDivide] = GUIKeyCode.KeypadDivide;
            map[(int)KeyCode.KeypadMultiply] = GUIKeyCode.KeypadMultiply;
            map[(int)KeyCode.KeypadMinus] = GUIKeyCode.KeypadMinus;
            map[(int)KeyCode.KeypadPlus] = GUIKeyCode.KeypadPlus;
            map[(int)KeyCode.KeypadEnter] = GUIKeyCode.KeypadEnter;
            map[(int)KeyCode.KeypadEquals] = GUIKeyCode.KeypadEquals;

            // --- ��������� ---
            map[(int)KeyCode.UpArrow] = GUIKeyCode.UpArrow;
            map[(int)KeyCode.DownArrow] = GUIKeyCode.DownArrow;
            map[(int)KeyCode.RightArrow] = GUIKeyCode.RightArrow;
            map[(int)KeyCode.LeftArrow] = GUIKeyCode.LeftArrow;
            map[(int)KeyCode.Insert] = GUIKeyCode.Insert;
            map[(int)KeyCode.Home] = GUIKeyCode.Home;
            map[(int)KeyCode.End] = GUIKeyCode.End;
            map[(int)KeyCode.PageUp] = GUIKeyCode.PageUp;
            map[(int)KeyCode.PageDown] = GUIKeyCode.PageDown;

            // --- �������������� ---
            map[(int)KeyCode.F1] = GUIKeyCode.F1;
            map[(int)KeyCode.F2] = GUIKeyCode.F2;
            map[(int)KeyCode.F3] = GUIKeyCode.F3;
            map[(int)KeyCode.F4] = GUIKeyCode.F4;
            map[(int)KeyCode.F5] = GUIKeyCode.F5;
            map[(int)KeyCode.F6] = GUIKeyCode.F6;
            map[(int)KeyCode.F7] = GUIKeyCode.F7;
            map[(int)KeyCode.F8] = GUIKeyCode.F8;
            map[(int)KeyCode.F9] = GUIKeyCode.F9;
            map[(int)KeyCode.F10] = GUIKeyCode.F10;
            map[(int)KeyCode.F11] = GUIKeyCode.F11;
            map[(int)KeyCode.F12] = GUIKeyCode.F12;
            map[(int)KeyCode.F13] = GUIKeyCode.F13;
            map[(int)KeyCode.F14] = GUIKeyCode.F14;
            map[(int)KeyCode.F15] = GUIKeyCode.F15;
            map[(int)KeyCode.F16] = GUIKeyCode.F16;
            map[(int)KeyCode.F17] = GUIKeyCode.F17;
            map[(int)KeyCode.F18] = GUIKeyCode.F18;
            map[(int)KeyCode.F19] = GUIKeyCode.F19;
            map[(int)KeyCode.F20] = GUIKeyCode.F20;
            map[(int)KeyCode.F21] = GUIKeyCode.F21;
            map[(int)KeyCode.F22] = GUIKeyCode.F22;
            map[(int)KeyCode.F23] = GUIKeyCode.F23;
            map[(int)KeyCode.F24] = GUIKeyCode.F24;

            // --- ������������ / ��������� ---
            map[(int)KeyCode.Numlock] = GUIKeyCode.Numlock;
            map[(int)KeyCode.CapsLock] = GUIKeyCode.CapsLock;
            map[(int)KeyCode.ScrollLock] = GUIKeyCode.ScrollLock;
            map[(int)KeyCode.RightShift] = GUIKeyCode.RightShift;
            map[(int)KeyCode.LeftShift] = GUIKeyCode.LeftShift;
            map[(int)KeyCode.RightControl] = GUIKeyCode.RightControl;
            map[(int)KeyCode.LeftControl] = GUIKeyCode.LeftControl;
            map[(int)KeyCode.RightAlt] = GUIKeyCode.RightAlt;
            map[(int)KeyCode.LeftAlt] = GUIKeyCode.LeftAlt;
            map[(int)KeyCode.RightCommand] = GUIKeyCode.RightCommand;
            map[(int)KeyCode.LeftCommand] = GUIKeyCode.LeftCommand;
            map[(int)KeyCode.LeftWindows] = GUIKeyCode.LeftWindows;
            map[(int)KeyCode.RightWindows] = GUIKeyCode.RightWindows;
            map[(int)KeyCode.AltGr] = GUIKeyCode.AltGr;
            map[(int)KeyCode.Help] = GUIKeyCode.Help;
            map[(int)KeyCode.Print] = GUIKeyCode.Print;
            map[(int)KeyCode.SysReq] = GUIKeyCode.SysReq;
            map[(int)KeyCode.Break] = GUIKeyCode.Break;
            map[(int)KeyCode.Menu] = GUIKeyCode.Menu;

            // --- ������ ���� ---
            map[(int)KeyCode.Mouse0] = GUIKeyCode.Mouse0;
            map[(int)KeyCode.Mouse1] = GUIKeyCode.Mouse1;
            map[(int)KeyCode.Mouse2] = GUIKeyCode.Mouse2;
            map[(int)KeyCode.Mouse3] = GUIKeyCode.Mouse3;
            map[(int)KeyCode.Mouse4] = GUIKeyCode.Mouse4;
            map[(int)KeyCode.Mouse5] = GUIKeyCode.Mouse5;
            map[(int)KeyCode.Mouse6] = GUIKeyCode.Mouse6;

            // --- ������� (����, 20 ������) ---
            map[(int)KeyCode.JoystickButton0] = GUIKeyCode.JoystickButton0;
            map[(int)KeyCode.JoystickButton1] = GUIKeyCode.JoystickButton1;
            map[(int)KeyCode.JoystickButton2] = GUIKeyCode.JoystickButton2;
            map[(int)KeyCode.JoystickButton3] = GUIKeyCode.JoystickButton3;
            map[(int)KeyCode.JoystickButton4] = GUIKeyCode.JoystickButton4;
            map[(int)KeyCode.JoystickButton5] = GUIKeyCode.JoystickButton5;
            map[(int)KeyCode.JoystickButton6] = GUIKeyCode.JoystickButton6;
            map[(int)KeyCode.JoystickButton7] = GUIKeyCode.JoystickButton7;
            map[(int)KeyCode.JoystickButton8] = GUIKeyCode.JoystickButton8;
            map[(int)KeyCode.JoystickButton9] = GUIKeyCode.JoystickButton9;
            map[(int)KeyCode.JoystickButton10] = GUIKeyCode.JoystickButton10;
            map[(int)KeyCode.JoystickButton11] = GUIKeyCode.JoystickButton11;
            map[(int)KeyCode.JoystickButton12] = GUIKeyCode.JoystickButton12;
            map[(int)KeyCode.JoystickButton13] = GUIKeyCode.JoystickButton13;
            map[(int)KeyCode.JoystickButton14] = GUIKeyCode.JoystickButton14;
            map[(int)KeyCode.JoystickButton15] = GUIKeyCode.JoystickButton15;
            map[(int)KeyCode.JoystickButton16] = GUIKeyCode.JoystickButton16;
            map[(int)KeyCode.JoystickButton17] = GUIKeyCode.JoystickButton17;
            map[(int)KeyCode.JoystickButton18] = GUIKeyCode.JoystickButton18;
            map[(int)KeyCode.JoystickButton19] = GUIKeyCode.JoystickButton19;

            return map;
        }
    }
}