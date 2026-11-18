export function section(title: string): HTMLElement {
    const element = document.createElement("section");
    const heading = document.createElement("h3");
    heading.textContent = title;
    element.appendChild(heading);
    document.body.appendChild(element);
    return element;
}

export function addButton(parent: HTMLElement, label: string): HTMLButtonElement {
    const button = document.createElement("button");
    button.textContent = label;
    parent.appendChild(button);
    return button;
}

export function addInput(parent: HTMLElement, type: string): HTMLInputElement {
    const input = document.createElement("input");
    input.type = type;
    parent.appendChild(input);
    return input;
}

export function addParagraph(parent: HTMLElement): HTMLParagraphElement {
    const paragraph = document.createElement("p");
    parent.appendChild(paragraph);
    return paragraph;
}
