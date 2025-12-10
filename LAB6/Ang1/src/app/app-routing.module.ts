import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SecondComponent } from './second/second.component';
import { FirstComponent } from './first/first.component';

const routes: Routes = [
  { path: '', component: FirstComponent, pathMatch: 'full' },
  { path: 'second', component: SecondComponent },
  { path: '**', component: FirstComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
