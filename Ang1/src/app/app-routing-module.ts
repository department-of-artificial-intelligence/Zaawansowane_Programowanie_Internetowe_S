// src/app/app-routing.module.ts

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
//import { HomeComponent } from './home/home.component';
//import { AboutComponent } from './about/about.component';
//import { ContactComponent } from './contact/contact.component';

const routes: Routes = [
  // Domyślne przekierowanie: po uruchomieniu aplikacji / powinien przekierować do /home
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  // Ścieżki do poszczególnych komponentów
  //{ path: 'home', component: HomeComponent },
  //{ path: 'about', component: AboutComponent },
  //{ path: 'contact', component: ContactComponent },
  // Opcjonalnie: obsługa nieznanych ścieżek (np. błąd 404)
  { path: '**', redirectTo: '/home' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
