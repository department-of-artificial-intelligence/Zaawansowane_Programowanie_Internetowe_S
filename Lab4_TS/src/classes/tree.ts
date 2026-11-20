import type { TreeTraverseAction } from "../types";

export class TreeNode<T>{
    private _children: TreeNode<T>[] = [];
    public value: T;

    get children(): TreeNode<T>[]{
        return this._children;
    }

    constructor(value: T){
        this.value = value;
    }

    addChild(node: TreeNode<T>): void{
        this._children.push(node);
    }

    removeChild(node: TreeNode<T>): void{
        const index = this._children.indexOf(node);
        if(index === -1) throw new Error(`Node: ${node}, is not a child of ${this}`);
        this._children.splice(index, 1);
    }
}

export class Tree<T>{
    private _root: TreeNode<T>;
    get root(): TreeNode<T>{
        return this._root;
    }

    constructor(root: TreeNode<T>){
        this._root = root;
    }

    public traverse(action: TreeTraverseAction<T>): void{
        const walk = (node: TreeNode<T>, depth: number): void => {
            action(node, depth);
            for(const child of node.children)
                walk(child, depth + 1);
        };
        walk(this._root, 0);
    }
}