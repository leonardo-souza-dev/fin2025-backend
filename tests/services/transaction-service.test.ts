import TransactionService from '../../src/services/transaction-service'
import SimpleTransactionService from '../../src/services/simple-transaction-service'
import TransferRepository from '../../src/infra/transfer-repository'
import Transaction from '../../src/models/transaction'

jest.mock('../../src/services/simple-transaction-service')
jest.mock('../../src/infra/transfer-repository')

describe('TransactionService - getAll', () => {
    let transactionService: TransactionService;
    let mockSimpleTransactionService: jest.Mocked<SimpleTransactionService>;
    let mockTransferRepository: jest.Mocked<TransferRepository>;

    beforeEach(() => {
        mockSimpleTransactionService = new SimpleTransactionService() as jest.Mocked<SimpleTransactionService>;
        mockTransferRepository = new TransferRepository() as jest.Mocked<TransferRepository>;
        transactionService = new TransactionService();

        (transactionService as any).simpleTransactionService = mockSimpleTransactionService;
        (transactionService as any).transferRepository = mockTransferRepository;
    });

    it('should return transactions from simple transactions and transfers', async () => {
        // Arrange
        mockSimpleTransactionService.getAllActive.mockResolvedValue([
            { date: '2023-01-01', description: 'Simple 1', accountId: 1, amount: 100, isActive: true, isRecurrent: false, id: 1 },
            { date: '2023-01-02', description: 'Simple 2', accountId: 2, amount: 200, isActive: true, isRecurrent: false,id: 2 },
        ]);

        mockTransferRepository.getAll.mockResolvedValue([
            { date: '2023-01-03', description: 'Transfer 1', sourceAccountId: 1, destinationAccountId: 2, amount: 300, isActive: true, isRecurrent: false, id: 3 },
            { date: '2023-01-04', description: 'Transfer 2', sourceAccountId: 2, destinationAccountId: 3, amount: 400, isActive: false, isRecurrent: false, id: 4 },
        ]);

        // Act
        const transactions = await transactionService.getAll();

        // Assert
        expect(transactions).toHaveLength(4);

        expect(transactions).toContainEqual(
            new Transaction('2023-01-01', 'Simple 1', 1, 100, 'SIMPLE', null, true, false, 1)
        );
        expect(transactions).toContainEqual(
            new Transaction('2023-01-02', 'Simple 2', 2, 200, 'SIMPLE', null, true, false, 2)
        );
        expect(transactions).toContainEqual(
            new Transaction('2023-01-03', 'Transfer 1', 1, 300, 'TRANSFER', 2, true, false, 3)
        );
    });

    it('should handle empty simple transactions and transfers', async () => {
        // Arrange
        mockSimpleTransactionService.getAllActive.mockResolvedValue([]);
        mockTransferRepository.getAll.mockResolvedValue([]);

        // Act
        const transactions = await transactionService.getAll();

        // Assert
        expect(transactions).toHaveLength(0);
    });

    it('should only include active transfers', async () => {
        // Arrange
        mockSimpleTransactionService.getAllActive.mockResolvedValue([]);
        mockTransferRepository.getAll.mockResolvedValue([
            { date: '2023-01-03', description: 'Transfer 1', sourceAccountId: 1, amount: 300, destinationAccountId: 2, isActive: true, isRecurrent: false, id: 3 },
            { date: '2023-01-04', description: 'Transfer 2', sourceAccountId: 2, amount: 400, destinationAccountId: 3, isActive: false, isRecurrent: false, id: 4 },
        ]);

        // Act
        const transactions = await transactionService.getAll();

        // Assert
        expect(transactions).toHaveLength(2);
    });
});