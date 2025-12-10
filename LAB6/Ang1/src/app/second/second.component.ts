import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-second',
  templateUrl: 'second.component.html',
  standalone: false,
  styleUrl: 'second.component.css',
})
export class SecondComponent implements OnInit {
  title: string = 'Komponent drugi!';
  person = {
    firstName: 'Ala',
    lastName: 'Nowak',
  };
  h1Class?: string;

  constructor() {}

  metoda() {
    return 'Wartośc zwrócona z metody';
  }
  ngOnInit(): void {}

  onClick() {
    this.title = 'Po kliknieciu';
    this.h1Class = 'Hoo';
  }
}
