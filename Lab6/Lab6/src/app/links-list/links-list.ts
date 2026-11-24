import { Component } from '@angular/core';

interface Link {
  name: string,
  value: string
}

@Component({
  selector: 'app-links-list',
  standalone: false,
  styleUrl: './links-list.css',
  templateUrl: './links-list.html',
})
export class LinksList {
  Links: Link[] = [];
  ClickedLinks = new Set<string>();

  constructor(){
    this.Links.push({name: "youtube", value: "https://www.youtube.com/"});
    this.Links.push({name: "wikipedia", value: "https://www.wikipedia.com/"});
    this.Links.push({name: "learnopengl", value: "https://learnopengl.com/"});
  }

  onLinkClick(link: Link, event: Event): void{
    event.preventDefault();
    this.ClickedLinks.clear()
    this.ClickedLinks.add(link.value);
  }

  isClicked(link: Link): boolean{
    return this.ClickedLinks.has(link.value);
  }
}
