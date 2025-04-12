import Entity from "./entity"

export default class User extends Entity<User> {

    constructor(
        public email: string, 
        public password: string, 
        public isActive: boolean, 
        public id: number | null) {
        super(id, isActive);
    }
}