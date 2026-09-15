import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent {
  name = '';
  query = '';
  submitted = false;

  sendQuery(form: NgForm): void {
    if (form.invalid) {
      return;
    }

    this.submitted = true;
    form.resetForm();
  }
}
