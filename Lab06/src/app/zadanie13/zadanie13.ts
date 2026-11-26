import { Component, computed, signal } from '@angular/core';

type ConversionResult =
  | { status: 'empty' }
  | { status: 'valid'; value: number }
  | { status: 'invalid' };

@Component({
  selector: 'app-zadanie13',
  imports: [],
  templateUrl: './zadanie13.html',
  styleUrl: './zadanie13.css',
})
export class Zadanie13 {
  protected readonly celsiusInput = signal('');

  protected readonly result = computed<ConversionResult>(() => {
    const trimmed = this.celsiusInput().trim();

    if (trimmed === '') {
      return { status: 'empty' };
    }

    const parsed = Number(trimmed.replace(',', '.'));
    if (!Number.isFinite(parsed)) {
      return { status: 'invalid' };
    }

    const fahrenheit = (parsed * 9) / 5 + 32;
    return {
      status: 'valid',
      value: Math.round(fahrenheit * 100) / 100,
    };
  });

  protected onInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.celsiusInput.set(value);
  }
}
