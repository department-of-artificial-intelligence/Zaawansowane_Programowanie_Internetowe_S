import { Tree, TreeNode } from "./classes/tree";
import { AutorContainer } from "./components/autorsContainer";
import { Counter } from "./components/counter";
import { AutorHTMLFormatter } from "./implementations/autorFormatters";

const node1 = new TreeNode(1);
const node2 = new TreeNode(2);
const node3 = new TreeNode(3);
const node4 = new TreeNode(4);
node1.addChild(node2);
node1.addChild(node3);
node3.addChild(node4);

const tree = new Tree(node1);
tree.traverse((node: TreeNode<number>, depth: number) => {
  console.log(`Depth: ${depth}, value: ${node.value}`);
});

const appContainer = document.getElementById("app") ?? document.body;

const counter = new Counter();
counter.mount(appContainer);

const autorHTMLFormatter = new AutorHTMLFormatter();
const autorsContainer = new AutorContainer(autorHTMLFormatter);
await autorsContainer.load();
autorsContainer.mount(appContainer);

const autorA = autorsContainer.findById(2);
console.log(`${autorA?.imie ?? ""} ${autorA?.nazwisko ?? ""}`.trim() || "Nie znaleziono autora");