import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-first',
  standalone: false,
  templateUrl: './first.component.html',
  styleUrl: './first.component.css'
})

export class FirstComponent {
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