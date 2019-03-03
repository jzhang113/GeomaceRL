using System;
using System.Collections.Generic;

namespace GeomaceRL
{
    // Generic tree implementation
    public class TreeNode<T>
    {
        public T Value { get; }
        public TreeNode<T> Parent { get; }
        public ICollection<TreeNode<T>> Children { get; }

        public TreeNode(T item)
        {
            Value = item;
            Parent = null;
            Children = new List<TreeNode<T>>();
        }

        public TreeNode(TreeNode<T> parent, T item)
        {
            Value = item;
            Parent = parent;
            Children = new List<TreeNode<T>>();
        }

        public void AddChild(T item)
        {
            Children.Add(new TreeNode<T>(this, item));
        }

        public void AddChild(TreeNode<T> node)
        {
            Children.Add(node);
        }

        public bool RemoveChild(TreeNode<T> node)
        {
            return Children.Remove(node);
        }

        public void Traverse(Action<T> action)
        {
            action(Value);
            foreach (var child in Children)
                child.Traverse(action);
        }
    }
}
