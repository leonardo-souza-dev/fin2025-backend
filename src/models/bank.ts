import Entity from "./entity"

export default class Bank extends Entity<Bank> {

    constructor(
        public name: string,
        public isActive: boolean, 
        public id: number | null) {
        super(id, isActive)
    }
}