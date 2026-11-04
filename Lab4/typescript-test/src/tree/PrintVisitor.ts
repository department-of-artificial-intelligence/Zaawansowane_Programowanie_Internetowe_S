import { TreeNode } from "./TreeNode";
import type { TreeVisitor } from "./TreeVisitor";

// Prints the tree with indentation
export class PrintVisitor<T> implements TreeVisitor<T> {
    private lines: string[] = [];
    private toText: (value: T) => string;

    constructor(toText: (value: T) => string = v => String(v)) {
        this.toText = toText;
    }

    visit(node: TreeNode<T>): void {
        this.lines.push("  ".repeat(node.depth) + "- " + this.toText(node.value));
    }

    getResult(): string {
        return this.lines.join("\n");
    }
}
