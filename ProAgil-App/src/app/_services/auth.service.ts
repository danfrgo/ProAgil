import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {JwtHelperService} from '@auth0/angular-jwt';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  
  baseURL = 'http://localhost:5000/api/user/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http
      .post(`${this.baseURL}login`, model).pipe(
        map((response: any) => { // tratar o que vou receber...tipo um mapeamento
          const user = response;
          if (user) {
            /* local stare é o armazenamente local que existe no browser...nao conseguimos acessar ao localstorage so apenas a mesma url
            consegue aceder ao localstorage, dai nao usar os cookies */
            localStorage.setItem('token', user.token);
            this.decodedToken = this.jwtHelper.decodeToken(user.token); // token que será decodificado
            sessionStorage.setItem('username', this.decodedToken.unique_name);
          }
        })
      );
  }
  register(model: any){
    return this.http.post(`${this.baseURL}register`, model);
  }

loggedIn(){
  const token = localStorage.getItem('token'); // vou receber o localstorage -> nome da chave é token
  return !this.jwtHelper.isTokenExpired(token); // se a data de validade do token estiver válida, entao o User está loggedIn
}


}
