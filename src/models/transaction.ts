import Entity from "./entity"

export default class Transaction extends Entity<Transaction> {

    constructor(
        public date: string, 
        public description: string, 
        public refAccountId: number,
        public amount: number,
        public type: string,
        public otherAccountId: number | null,
        public isActive: boolean, 
        public isRecurrent: boolean,
        public id: number | null) {
        super(id, isActive);
    }
}