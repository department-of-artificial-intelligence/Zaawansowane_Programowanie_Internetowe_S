import { Component, signal } from '@angular/core';

@Component({
  imports: [],
  selector: 'app-first',
  styleUrl: './first.css',
  templateUrl: './first.html',
  
})
export class First {
  
  protected readonly title = signal("Pierwsza wartość przekazana z komponentu!");
  protected readonly h1Class = signal({});
  

  protected readonly person = {
    firstName: "Ala",
    lastName: "Nowak"
  };

  protected method() {
    return "Wartość zwrócona z metody";
  }

  protected onClick() {
    this.title.set("Po kliknięciu");
    this.h1Class.set({"selected":true, "clicked":false});
    
  }
  
}
