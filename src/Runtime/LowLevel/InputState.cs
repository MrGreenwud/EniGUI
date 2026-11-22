using UnityEngine;

namespace EniGUI
{
    public readonly struct InputState
    {
        public readonly struct Chord
        {
            internal readonly GUIKeyCode Key;
            internal readonly GUIInputTracker.KeySet Modifiers;

            public Chord(in GUIKeyCode key, in GUIInputTracker.KeySet modifiers)
            {
                Key = key;
                Modifiers = modifiers;
            }

            public static Chord Build(GUIKeyCode key, params GUIKeyCode[] modifiers)
            {
                var set = new GUIInputTracker.KeySet();

                foreach (var modifier in modifiers)
                    set.Set(modifier);

                return new(key, set);
            }
        }

        private readonly GUIInputTracker.Input m_Input;

        public InputState(GUIInputTracker.Input input)
        {
            m_Input = input;
        }

        public Vector2 Pointer => m_Input.Pointer;
        public Vector2 PointerDelta => m_Input.PointerDelta;
        public Vector2 Scroll => m_Input.Scroll;

        public bool Held(GUIKeyCode key) => m_Input.Held.Has(key);
        public bool Pressed(GUIKeyCode key) => m_Input.Pressed.Has(key);
        public bool Released(GUIKeyCode key) => m_Input.Released.Has(key);

        public bool Shortcut(in Chord chord)
        {
            return m_Input.Held.HasAll(chord.Modifiers)
                   && m_Input.Pressed.Has(chord.Key);
        }
    }
}