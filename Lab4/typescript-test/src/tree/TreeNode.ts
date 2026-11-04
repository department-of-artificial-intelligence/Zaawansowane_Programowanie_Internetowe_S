import type { TreeVisitor } from "./TreeVisitor";

export class TreeNode<T> {
    private _children: TreeNode<T>[] = [];
    private _parent: TreeNode<T> | null = null;
    private _value: T;

    constructor(value: T) {
        this._value = value;
    }

    get value(): T { return this._value; }
    set value(value: T) { this._value = value; }

    get parent(): TreeNode<T> | null { return this._parent; }

    get children(): TreeNode<T>[] { return [...this._children]; }

    get isLeaf(): boolean { return this._children.length === 0; }

    get depth(): number {
        return this._parent === null ? 0 : this._parent.depth + 1;
    }

    addChild(value: T): TreeNode<T> {
        const child = new TreeNode(value);
        child._parent = this;
        this._children.push(child);
        return child;
    }

    removeChild(child: TreeNode<T>): boolean {
        const index = this._children.indexOf(child);
        if (index === -1) return false;
        this._children.splice(index, 1);
        child._parent = null;
        return true;
    }

    // Visitor pattern - the node "accepts" the visitor and passes it on to its children
    accept(visitor: TreeVisitor<T>): void {
        visitor.visit(this);
        for (const child of this._children) {
            child.accept(visitor);
        }
    }
}
