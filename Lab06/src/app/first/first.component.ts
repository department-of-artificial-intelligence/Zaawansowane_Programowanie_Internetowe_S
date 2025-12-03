import { Component } from '@angular/core';

@Component({
  selector: 'app-first',
  standalone: false,
  templateUrl: './first.component.html',
  styleUrl: './first.component.css',
})
export class FirstComponent {
  name: string = 'Imię i nazwisko';

  constructor() {}

  method() {
    return 'wartosc';
  }

  onClick() {
    this.name = 'Adam Nowak';
    console.log('kiknieto');
  }
}
