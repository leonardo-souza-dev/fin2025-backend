export default class Entity<T> {
    
    constructor(
        public id: number | null,
        public isActive: boolean) {
    }
}