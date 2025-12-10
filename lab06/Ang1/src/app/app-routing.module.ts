import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FirstComponent }from './first/first.component';
import { SecendComponent }from './secend/secend.component';

const routes: Routes = [
  { path: "", component: FirstComponent, pathMatch: "full" },////strona glowna
  { path: "blue", component: FirstComponent },///// strona glowna na profile
  { path: "green", component: SecendComponent },
  { path: "**", component: SecendComponent } /// kazdy adres przechwytuje
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
