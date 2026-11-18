export class TreeNode<T> {
    private _value: T;
    private _children: TreeNode<T>[] = [];

    constructor(value: T) {
        this._value = value;
    }

    get value(): T { return this._value; }

    get children(): TreeNode<T>[] { return this._children; }

    addChild(value: T): TreeNode<T> {
        const child = new TreeNode<T>(value);
        this._children.push(child);
        return child;
    }
}
