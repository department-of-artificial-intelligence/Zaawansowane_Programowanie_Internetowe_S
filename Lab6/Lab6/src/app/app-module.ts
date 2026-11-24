import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule, provideClientHydration } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { First } from './first/first';
import { MaterialModule } from './material/material-module';
import { LinksList } from './links-list/links-list';
import { Counter } from './counter/counter';
import { ImageGallery } from './image-gallery/image-gallery';

@NgModule({
  declarations: [App, First, LinksList, Counter, ImageGallery],
  imports: [BrowserModule, AppRoutingModule, MaterialModule],
  providers: [provideBrowserGlobalErrorListeners(), provideClientHydration()],
  bootstrap: [App],
})
export class AppModule {}
