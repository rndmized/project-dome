
/** Defining User Class
 * 
 * User class holds data relevant to the user stored in the database.
 * It is used to represent the values of the User and display them to the 
 * Manager.
*/
export class User {
    _id:string;
    username: string;
    full_name: string;
    email:string;
    password:string;
    status:string;
    admin:boolean;
  }

  /** Database excerpt */
  
  /*
  "_id": "5abe3a69bf427311e43b74cb",
  "username": "rndmized",
  "full_name": "Albert",
  "email": "rndmized@project-dome.ie",
  "password": "password",
  "status": "Allowed"
  "admin": false,
  "__v": 0
  */