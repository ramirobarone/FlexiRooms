import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class AppComponent implements OnInit {
  title = 'Flexi Rooms';

  userName: string | null = '';

  constructor() {

   
  }
  ngOnInit(): void {
    // window.addEventListener('refrescar', () => {
    //   console.log('eventoStorage');
    //   this.userName = '';
    //   this.userName = localStorage.getItem('fullname');
    //   console.log('eventoStorage', this.userName);

    // });
  }

}
