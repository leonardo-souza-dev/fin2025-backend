import Entity from "./entity"

export default class Recurrence extends Entity<Recurrence> {

    constructor(
        public name: string, 
        public startYearMonth: string, 
        public endYearMonth: string,
        public day: number,
        public accountId: number,
        public amount: number,
        public isActive: boolean,
        public id: number | null) {
        super(id, isActive);
    }
}