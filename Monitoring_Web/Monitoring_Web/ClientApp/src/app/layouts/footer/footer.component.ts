import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {

  constructor() { }
  year = ""
  currentYear: any
  ngOnInit(): void {
    currentYear: Date;
    this.currentYear = new Date().getFullYear();
    if( this.currentYear == "2022"){
      this.year = "2022"
    }
    else{
      this.year = "2022 - " + this.currentYear
    }
    
  }
  scrollTop() {
    window.scroll({
      top: 0,
      left: 0,
      behavior: 'smooth'
    });

  }
}
