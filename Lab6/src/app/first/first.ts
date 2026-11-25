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
  style = { "font-size.px": 12, "color": "red" }

  protected method() {
    return "Wartość zwrócona z metody";
  }
  protected onClick() {
    this.title.set("Po kliknięciu");
    this.h1Class.set({ "selected": true, "clicked": false });


  }
}