import Entity from "./entity"

export default class SimpleTransaction extends Entity<SimpleTransaction> {

    constructor(
        public date: string, 
        public description: string, 
        public accountId: number,
        public amount: number,
        public isActive: boolean, 
        public isRecurrent: boolean,
        public id: number | null) {
        super(id, isActive);
    }
}