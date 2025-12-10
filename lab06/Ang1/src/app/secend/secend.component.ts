import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-first',
  standalone: false,
  templateUrl: './secend.component.html',
  styleUrl: './secend.component.css'
})

export class SecendComponent {
  title: string = "Pierwsza wartosc przekazana z komponentu";
  person = {
    firstName: "Ala",
    lastName: "Nowak"
  };
  constructor() { }
  method() {
    return "Wartosc zwrocona z metody";
  }

  onClick() {
    this.title = "person";
  }
}