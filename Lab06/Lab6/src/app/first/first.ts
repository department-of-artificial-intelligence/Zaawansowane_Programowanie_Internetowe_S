import { Component, signal } from '@angular/core';
@Component({
selector: 'app-first',
imports: [],
templateUrl: './first.html',
styleUrl: './first.css',
})
export class First {

  protected readonly h1Class = signal({});
  protected readonly title = signal("Pierwsza wartość przekazana z komponentu");
  headerColor = signal('red');
  style = signal('color: blue; font-size: 30px;');


  protected readonly person = {
    firstName: "Ala",
    lastName: "Nowak"
  };

  method() {
    return "Wartość zwrócona z metody";
  }

  protected onClick() {
    this.title.set("Po kliknięciu");
    this.h1Class.set({"selected":true, "clicked":false});

  }
}
