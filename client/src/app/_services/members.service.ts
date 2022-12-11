import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }
  members: Member[] = []


  getMembers(){
    //if there are currently members in the array, return it as an observable
    if(this.members.length > 0) return of(this.members);
    //otherwise get members from the api URL
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members
        return members
      })
    )
  }

  getMember(username: string){
    const member = this.members.find(x => x.userName === username);
    if (member) return of (member);
    return this.http.get<Member>(this.baseUrl + 'users/'+username)
  
  }

  updateMember(member: Member) {
   return this.http.put(this.baseUrl + 'users', member).pipe(
    map(() => {
      const index = this.members.indexOf(member);
      // the ... operator takes the member thats available at that index and spreads all the info (id/username/knowas etc)
      this.members[index] = {...this.members[index], ...member}
    })
   )
  }

}
