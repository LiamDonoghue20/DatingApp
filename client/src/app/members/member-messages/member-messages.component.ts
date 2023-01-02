import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm
  //because its a child of the member detail component, we can grab the username for this child component via input
  @Input() username?: string;
  //messages as an input as they will only now be loaded via the parent (member detail) component when the messages tab is selcted
  @Input() messages: Message[] = [];
  messageContent = '';


  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
  }

  sendMessage() {
    if(!this.username) return;

    this.messageService.sendMessage(this.username, this.messageContent).subscribe({
      next: message => {
        this.messages.push(message);
        this.messageForm.reset
      }
    })
  }
  

}
