import type { ITreeVisitor } from "../interfaces/ITreeVisitor";

export class TreeNode<T> {
  private _value: T;
  private _children: TreeNode<T>[] = [];

  constructor(value: T) {
    this._value = value;
  }

  get value(): T {
    return this._value;
  }

  set value(val: T) {
    this._value = val;
  }

  get children(): TreeNode<T>[] {
    return this._children;
  }

  public addChild(child: TreeNode<T>): void {
    this._children.push(child);
  }

  public accept(visitor: ITreeVisitor<T>): void {
    visitor.visit(this);
    for (const child of this._children) {
      child.accept(visitor);
    }
  }

  public traverse(action: (value: T) => void): void {
    action(this._value);
    this._children.forEach((child) => child.traverse(action));
  }
}

export class Tree<T> {
  private _root: TreeNode<T>;

  constructor(rootValue: T) {
    this._root = new TreeNode<T>(rootValue);
  }

  get root(): TreeNode<T> {
    return this._root;
  }

  public accept(visitor: ITreeVisitor<T>): void {
    this._root.accept(visitor);
  }

  public traverse(action: (value: T) => void): void {
    this._root.traverse(action);
  }
}

export class ConsolePrintVisitor<T> implements ITreeVisitor<T> {
  visit(node: TreeNode<T>): void {
    console.log("Odwiedzono węzeł:", node.value);
  }
}
