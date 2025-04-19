import Entity from "./entity"

export default class Account extends Entity<Account> {

    constructor(
        public name: string,
        public isActive: boolean, 
        public comments: string,
        public id: number | null) {
        super(id, isActive)        
    }
}