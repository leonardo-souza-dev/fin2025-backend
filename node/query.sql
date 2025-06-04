-- account balance in a specific date
WITH params AS (SELECT 16 AS account_id, '2025-04-03' AS cutoff_date),
simple_trans AS (SELECT sum(amount) AS amt FROM simpleTransactions WHERE accountId = (SELECT account_id FROM params) AND isActive = 1 AND date <= (SELECT cutoff_date FROM params)),
out_trans AS (SELECT sum(amount) AS amt FROM transfers WHERE sourceAccountId = (SELECT account_id FROM params) AND isActive = 1 AND date <= (SELECT cutoff_date FROM params)),
in_trans AS (SELECT sum(amount) * -1 AS amt FROM transfers WHERE destinationAccountId = (SELECT account_id FROM params) AND isActive = 1 AND date <= (SELECT cutoff_date FROM params))
SELECT COALESCE((SELECT amt FROM simple_trans), 0) + COALESCE((SELECT amt FROM out_trans), 0) + COALESCE((SELECT amt FROM in_trans), 0) AS total_amount;

------------------
--    BALANCE
------------------
select sum(amount) as balance from (
--select * from (
	select st.date, st.description, st.accountId, st.amount, 'simple' as type from simpleTransactions st
	inner join accounts a on a.id = st.accountId
		where st.date <= '2025-03-04' and -- ⏪ DATE
		a.id = 17 and -- ⏪ ACCOUNT ID
		st.isActive = 1
	union all
	select t.date, t.description, t.sourceAccountId as accountId, t.amount , 'out' as type from transfers t
	inner join accounts a on a.id = t.sourceAccountId
	where t.date <= '2025-03-04' and -- ⏪ DATE
		a.id = 17 and -- ⏪ ACCOUNT ID
		t.isActive = 1
	union all
	select t.date, t.description, t.sourceAccountId as accountId, t.amount * -1, 'in' as type from transfers t
	inner join accounts a on a.id = t.destinationAccountId
	where t.date <= '2025-03-04' and -- ⏪ DATE
		a.id = 17 and -- ⏪ ACCOUNT ID
		t.isActive = 1
) as g
order by g.date desc

-- get count of days since first transaction until today, counting today
SELECT floor(julianday(date('now'))) - floor(julianday(min(date))) + 1 as CountDaysSinceFirstUntilToday FROM simpleTransactions

-- faz a comparação entre simpleTransaction e coupon
SELECT 
	c.simpleTransactionId, 
	(COALESCE(sum(c.itemPrice),0) - COALESCE(sum(c.itemDiscount),0)) as CouponValue, 
	st.amount, 
	COALESCE(ROUND(sum(c.itemPrice) * 100) - COALESCE(sum(c.itemDiscount) * 100,0), 0) as CouponValeu100, 
	COALESCE(ROUND(st.amount * 100), 0) as SimpleTransactionValue100, 
	(
		COALESCE(ROUND(sum(c.itemPrice)*100),0) - COALESCE(sum(c.itemDiscount) * 100,0) + COALESCE(ROUND(st.amount*100), 0)
	)/100 as PROVA 
FROM simpleTransactions st
INNER JOIN coupons c on c.simpleTransactionId = st.id
GROUP BY c.simpleTransactionId, st.amount




select * from accounts

select * from transfers where destinationAccountId = 16

select (sum(itemPrice) - sum(itemDiscount)) from coupouns where otherTransaction = 'ALELO, 2025-02-11'

select sum(itemPrice) - sum(itemDiscount) from coupouns where simpleTransactionId = 1014

select sum(itemPrice) from coupouns where simpleTransactionId = 1014