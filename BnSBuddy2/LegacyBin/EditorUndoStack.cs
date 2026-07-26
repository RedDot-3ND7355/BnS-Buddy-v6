using System;
using System.Collections.Generic;

namespace BnSBuddy2.LegacyBin
{
    public interface IEditorUndoAction
    {
        string Description { get; }
        void Undo();
        void Redo();
    }

    /// <summary>Simple undo/redo stack for editor actions.</summary>
    public sealed class EditorUndoStack
    {
        private readonly Stack<IEditorUndoAction> _undo = new Stack<IEditorUndoAction>();
        private readonly Stack<IEditorUndoAction> _redo = new Stack<IEditorUndoAction>();
        private const int MaxDepth = 100;

        public bool CanUndo => _undo.Count > 0;
        public bool CanRedo => _redo.Count > 0;
        public string UndoDescription => _undo.Count > 0 ? _undo.Peek().Description : null;
        public string RedoDescription => _redo.Count > 0 ? _redo.Peek().Description : null;

        public event Action Changed;

        public void Push(IEditorUndoAction action)
        {
            if (action == null)
            {
                return;
            }
            _undo.Push(action);
            if (_undo.Count > MaxDepth)
            {
                // Drop oldest: stack.ToArray is newest-first
                var arr = _undo.ToArray();
                _undo.Clear();
                for (int i = Math.Min(arr.Length, MaxDepth) - 1; i >= 0; i--)
                {
                    _undo.Push(arr[i]);
                }
            }
            _redo.Clear();
            Changed?.Invoke();
        }

        public void Undo()
        {
            if (_undo.Count == 0)
            {
                return;
            }
            var a = _undo.Pop();
            a.Undo();
            _redo.Push(a);
            Changed?.Invoke();
        }

        public void Redo()
        {
            if (_redo.Count == 0)
            {
                return;
            }
            var a = _redo.Pop();
            a.Redo();
            _undo.Push(a);
            Changed?.Invoke();
        }

        public void Clear()
        {
            _undo.Clear();
            _redo.Clear();
            Changed?.Invoke();
        }
    }
}
