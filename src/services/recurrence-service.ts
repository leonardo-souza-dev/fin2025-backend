import RecurrenceRepository from '../infra/recurrence-repository'
import Recurrence from '../models/recurrence'

export default class RecurrenceService {

    private recurrenceRepository: RecurrenceRepository = new RecurrenceRepository()

    async getAllActive(yearDashMonth: string): Promise<Recurrence[]> {

        const recurrences = await this.recurrenceRepository.getAllActive()

        const recurrencesFiltered = recurrences.filter(r => {
            return r.startYearMonth <= yearDashMonth && r.endYearMonth >= yearDashMonth
        })

        return recurrencesFiltered
    }
}