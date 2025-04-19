import Entity from "./entity"

export default class Config  extends Entity<Config> {

    constructor(
        public key: string,
        public value: string,
        public isActive: boolean, 
        public id: number | null) {
        super(id, isActive)
    }
}