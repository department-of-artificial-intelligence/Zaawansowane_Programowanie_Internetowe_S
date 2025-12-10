import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { Routes, RouterModule } from '@angular/router';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FirstComponent } from './first/first.component';
import { SecondComponent } from './second/second.component';
import { ContactListComponent } from './contact-list/contact-list.component';

const routes: Routes = [];

@NgModule({
  declarations: [AppComponent, FirstComponent, SecondComponent, ContactListComponent],
  imports: [BrowserModule, AppRoutingModule, RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
