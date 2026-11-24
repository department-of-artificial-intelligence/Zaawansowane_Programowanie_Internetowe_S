import { Component } from '@angular/core';

@Component({
  selector: 'app-first',
  standalone: false,
  styleUrls: [`first.css`],
  templateUrl: `first.html`,
})
export class First {
  pClasses: string[] = [];

  me = {
    name: "",
    surname: ""
  }

  onShowMe(){
    this.me.name = "Erik";
    this.me.surname = "Steshenko";
  }

  onSetFont(){
    if(!this.pClasses.includes("big-font")){
      this.pClasses = [...this.pClasses, "big-font"];
    }
    else{
      this.pClasses = this.pClasses.filter(c => c !== "big-font");
    }
  }

  onSetStyle(){
    if(!this.pClasses.includes("green-color")){
      this.pClasses = [...this.pClasses, "green-color", "crossed-out"];
    }
    else{
      this.pClasses = this.pClasses.filter(c => c !== "green-color" && c !== "crossed-out");
    }
  }
}
