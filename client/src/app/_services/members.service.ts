import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginationHeaders, getPaginatedResult } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = []
  memberCache = new Map();
  user: User | undefined;
  userParams: UserParams | undefined;

  constructor(private http: HttpClient, private accountService: AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    })

  }

  getUserParams(){
    return this.userParams;
  }

  setUserParams(params: UserParams){
    this.userParams = params;
  }

  resetUserParams(){
    if(this.user){
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
  }

  getMembers(userParams: UserParams){
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if(response)  return of(response)
     //sets the params from the http request
    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
    //Call get to users endpoint and observe the response to map the pagination settings into the params variable
    return getPaginatedResult<Member[]>(this.baseUrl+'users',params, this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response); 
        return response;
      })
    )
  }

  getMember(username: string){
    const member =  [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === username)

    if (member) return of(member)

    //if we dont have it in our cache grab it from the API
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

  setMainPhoto(photoId: number){
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl + 'users/delete-photo/'+ photoId);
  }
  
  addLike(username: string){
    return this.http.post(this.baseUrl + 'likes/'+ username, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number){
    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('predicate', predicate)
    return getPaginatedResult<Member[]>(this.baseUrl+'likes', params, this.http);
  }
  
}
