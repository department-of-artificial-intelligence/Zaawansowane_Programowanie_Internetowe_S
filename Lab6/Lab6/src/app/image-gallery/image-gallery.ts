import { Component } from '@angular/core';

interface GalleryImage {
  name: string,
  link: string
}

@Component({
  selector: 'app-image-gallery',
  standalone: false,
  styleUrl: './image-gallery.css',
  templateUrl: './image-gallery.html',
})
export class ImageGallery {
  Images: GalleryImage[] = [
    { name: "field", link: "https://eu.startpage.com/av/proxy-image?piurl=https%3A%2F%2Fwallpaperaccess.com%2Ffull%2F138842.jpg&sp=1795542606T4baa53532215f32f9bf71b2d96ff8aeb7e8b13e478f29374b5f836e8d63de0ef"},
    { name: "lake", link: "https://eu.startpage.com/av/proxy-image?piurl=https%3A%2F%2Fwallpapercave.com%2Fwp%2Fwp4406701.jpg&sp=1795542606T39b49e238c5f89dbaf90d64375cd3b874f07c881d1e103e3e089fc4ae6add6fc"},
    { name: "sea", link: "https://eu.startpage.com/av/proxy-image?piurl=https%3A%2F%2Fwww.pixelstalk.net%2Fwp-content%2Fuploads%2F2016%2F06%2FFull-hd-nature-wallpapers-1080p.jpg&sp=1795542606T328b2448b4667bbdf80196522f86f0bdd06ac9bde0bee51347861d6502015638"}
  ];

  CurrentImage: GalleryImage = this.Images[0];

  ShowCantGoPreviousMessage: boolean = false;
  ShowCantGoNextMessage: boolean = false;

  OnLastImage: boolean = false;
  OnFirstImage: boolean = true;

  onPreviousImage(){
    let currentImageIndex = this.Images.indexOf(this.CurrentImage);
    if(currentImageIndex === 0){
      this.ShowCantGoPreviousMessage = true;
      return;
    }
    this.CurrentImage = this.Images[currentImageIndex - 1];
    this.ShowCantGoPreviousMessage = false;
    this.ShowCantGoNextMessage = false;
    this.OnLastImage = false;
    if(currentImageIndex - 1 === 0){
      this.OnFirstImage = true;
    }
  }

  onNextImage(){
    let currentImageIndex = this.Images.indexOf(this.CurrentImage);
    if(currentImageIndex === this.Images.length - 1){
      this.ShowCantGoNextMessage = true;
      return;
    }
    this.CurrentImage = this.Images[currentImageIndex + 1];
    this.ShowCantGoPreviousMessage = false;
    this.ShowCantGoNextMessage = false;
    this.OnFirstImage = false;
    if(currentImageIndex + 1 === this.Images.length - 1){
      this.OnLastImage = true;
    }
  }
}
