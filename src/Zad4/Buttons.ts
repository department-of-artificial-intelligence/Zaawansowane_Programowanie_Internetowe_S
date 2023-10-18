
export class Buttons {
    private inputValue:number = 0;
    private input:HTMLInputElement;
    private plusButton:HTMLButtonElement;
    private minusButton:HTMLButtonElement;
    private resultElement:HTMLDivElement;

    constructor(){
        this.input = document.getElementById('firstInput') as HTMLInputElement;
        this.plusButton = document.getElementById('plusButton') as HTMLButtonElement;
        this.minusButton = document.getElementById('minusButton') as HTMLButtonElement;
        this.resultElement = document.getElementById('minusButton') as HTMLDivElement;

        this.plusButton.addEventListener('click',this.incrementFunction.bind(this));
        this.minusButton.addEventListener('click',this.decrementFunction.bind(this));
        this.updateResult();
    }

    private incrementFunction(){
        this.inputValue++;
        this.updateResult();
    }

    private decrementFunction(){
        if(this.inputValue > 0){
            this.inputValue--;
            this.updateResult();
        }
    }
    

    private updateResult() {
        this.input.value = this.inputValue.toString();
        this.resultElement.textContent = `Wartość: ${this.inputValue}`;
    }

    

}

const app = new Buttons();