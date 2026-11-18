import { TreeNode } from "./TreeNode";

export class Tree<T> {
    private _root: TreeNode<T>;

    constructor(rootValue: T) {
        this._root = new TreeNode<T>(rootValue);
    }

    get root(): TreeNode<T> { return this._root; }
}
