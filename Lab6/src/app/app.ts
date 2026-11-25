
import { Component } from '@angular/core';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NgClass],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  imieNazwisko: string = '';

  aktualnaKlasa: string = '';

  pokazDane(): void {
    this.imieNazwisko = 'Paweł Szczypior';
  }

  zmienNaWiekszaCzcionke(): void {
    this.aktualnaKlasa = 'wieksza-czcionka';
  }

  zmienNaZielony(): void {
    this.aktualnaKlasa = 'zielony-przekreslony';
  }
}
