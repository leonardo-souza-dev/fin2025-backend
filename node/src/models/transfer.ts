import Entity from "./entity"

export default class Transfer extends Entity<Transfer> {

    constructor(
        public date: string, 
        public description: string, 
        public sourceAccountId: number,
        public amount: number,
        public isActive: boolean, 
        public isRecurrent: boolean,
        public destinationAccountId: number,
        public id: number | null) {
        super(id, isActive);
    }
}