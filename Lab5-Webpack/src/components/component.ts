export abstract class Component {
    private _wrapper: HTMLDivElement;

    constructor(){
        this._wrapper = document.createElement("div");
    }

    public mount(container: HTMLElement){
        container.appendChild(this._wrapper);
    }

    protected appendChild(child: HTMLElement){
        this._wrapper.appendChild(child);
    }
}