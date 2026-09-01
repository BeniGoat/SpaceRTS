namespace SpaceRTS.Models
{
	/// <summary>
	/// Maintains explicit state for orbital slots and provides validated transitions
	/// between free, reserved, and occupied states.
	/// </summary>
	public sealed class OrbitalSlotRegistry
	{
		private readonly SlotState[] slots;
		private readonly int[] reservationTokens;
		private int nextReservationToken;

		/// <summary>
		/// Initialises a new instance of the <see cref="OrbitalSlotRegistry"/> class with the specified number of slots.
		/// </summary>
		/// <param name="slotCount">The number of orbital slots to manage.</param>
		/// <exception cref="global::System.ArgumentOutOfRangeException">Thrown if <paramref name="slotCount"/> is less than or equal to zero.</exception>
		public OrbitalSlotRegistry(int slotCount)
		{
			if (slotCount <= 0)
				throw new global::System.ArgumentOutOfRangeException(nameof(slotCount));

			// Initialize the slots and reservation tokens arrays
			this.slots = new SlotState[slotCount];
			this.reservationTokens = new int[slotCount];
			for (int i = 0; i < this.reservationTokens.Length; i++)
			{
				this.reservationTokens[i] = -1;
			}
		}
		
		/// <summary>
		/// Gets the total number of orbital slots managed by this registry.
		/// </summary>
		public int SlotCount => this.slots.Length;

		/// <summary>
		/// Gets the number of currently occupied orbital slots.
		/// </summary>
		public int OccupiedCount { get; private set; }

		/// <summary>
		/// Gets the number of currently reserved orbital slots.
		/// </summary>
		public int ReservedCount { get; private set; }

		/// <summary>
		/// Gets the total number of committed (occupied + reserved) orbital slots.
		/// </summary>
		public int CommittedCount => this.OccupiedCount + this.ReservedCount;

		/// <summary>
		/// Gets a value indicating whether there are available (free) orbital slots.
		/// </summary>
		public bool HasAvailableSlots => this.CommittedCount < this.SlotCount;

		/// <summary>
		/// Attempts to reserve an available orbital slot.
		/// </summary>
		/// <param name="reservation">When this method returns, contains the reservation handle if the reservation was successful; otherwise, <see cref="OrbitalSlotReservation.None"/>.</param>
		/// <returns><c>true</c> if a slot was successfully reserved; otherwise, <c>false</c>.</returns>
		public bool TryReserve(out OrbitalSlotReservation reservation)
		{
			for (int i = 0; i < this.slots.Length; i++)
			{
				if (this.slots[i] != SlotState.Free)				
					continue;

				// Reserve the slot and generate a unique token for this reservation
				this.slots[i] = SlotState.Reserved;
				int token = ++this.nextReservationToken;
				this.reservationTokens[i] = token;
				this.ReservedCount++;
				reservation = new OrbitalSlotReservation(i, token);
				return true;
			}

			reservation = OrbitalSlotReservation.None;
			return false;
		}

		/// <summary>
		/// Releases a previously reserved orbital slot.
		/// </summary>
		/// <param name="reservation">The reservation handle to release.</param>
		/// <returns><c>true</c> if the reservation was successfully released; otherwise, <c>false</c>.</returns>
		public bool ReleaseReservation(OrbitalSlotReservation reservation)
		{
			// Validate the reservation before releasing it
			if (!this.IsValidReservation(reservation))			
				return false;

			// Release the reservation and mark the slot as free
			this.slots[reservation.SlotIndex] = SlotState.Free;
			this.reservationTokens[reservation.SlotIndex] = -1;
			this.ReservedCount--;
			return true;
		}

		/// <summary>
		/// Commits a valid orbital slot reservation and marks the slot as occupied.
		/// </summary>
		/// <remarks>If the reservation is invalid, no slot state or counters are changed.</remarks>
		/// <param name="reservation">Reservation to commit.</param>
		/// <param name="occupiedSlot">When this method returns <see langword="true"/>, contains the occupied slot created from the reservation;
		/// otherwise, <see cref="OrbitalOccupiedSlot.None"/>.</param>
		/// <returns><see langword="true"/> if the reservation is valid and is committed; otherwise, <see langword="false"/>.</returns>
		public bool TryCommitReservation(
			OrbitalSlotReservation reservation,
			out OrbitalOccupiedSlot occupiedSlot)
		{
			// Validate the reservation before committing it
			if (!this.IsValidReservation(reservation))
			{
				occupiedSlot = OrbitalOccupiedSlot.None;
				return false;
			}

			// Commit the reservation and mark the slot as occupied
			this.slots[reservation.SlotIndex] = SlotState.Occupied;
			this.reservationTokens[reservation.SlotIndex] = -1;
			this.ReservedCount--;
			this.OccupiedCount++;
			occupiedSlot = new OrbitalOccupiedSlot(reservation.SlotIndex);
			return true;
		}

		/// <summary>
		/// Attempts to reserve and commit the next available orbital slot in a single operation.
		/// </summary>
		/// <param name="occupiedSlot">When this method returns <see langword="true"/>, contains the occupied slot created from the reservation;
		/// otherwise, <see cref="OrbitalOccupiedSlot.None"/>.</param>
		/// <returns><see langword="true"/> if a slot was successfully reserved and committed; otherwise, <see langword="false"/>.</returns>
		public bool TryOccupyNext(out OrbitalOccupiedSlot occupiedSlot)
		{
			// Attempt to reserve the next available slot
			if (!this.TryReserve(out OrbitalSlotReservation reservation))
			{
				occupiedSlot = OrbitalOccupiedSlot.None;
				return false;
			}

			// Commit the reservation and mark the slot as occupied
			return this.TryCommitReservation(reservation, out occupiedSlot);
		}

		/// <summary>
		/// Releases a previously occupied orbital slot.
		/// </summary>
		/// <param name="occupiedSlot">The occupied slot to release.</param>
		/// <returns><c>true</c> if the occupied slot was successfully released; otherwise, <c>false</c>.</returns>
		public bool ReleaseOccupied(OrbitalOccupiedSlot occupiedSlot)
		{
			// Validate the occupied slot before releasing it
			if (!this.IsValidSlotIndex(occupiedSlot.SlotIndex)
				|| this.slots[occupiedSlot.SlotIndex] != SlotState.Occupied)			
				return false;

			// Release the occupied slot and mark it as free
			this.slots[occupiedSlot.SlotIndex] = SlotState.Free;
			this.OccupiedCount--;
			return true;
		}

		/// <summary>
		/// Validates whether the provided reservation is valid for the current state of the registry.
		/// </summary>
		/// <param name="reservation">The reservation to validate.</param>
		/// <returns><c>true</c> if the reservation is valid; otherwise, <c>false</c>.</returns>
		private bool IsValidReservation(OrbitalSlotReservation reservation) 
			=> this.IsValidSlotIndex(reservation.SlotIndex)
			&& this.slots[reservation.SlotIndex] == SlotState.Reserved
			&& this.reservationTokens[reservation.SlotIndex] == reservation.Token;

		/// <summary>
		/// Determines whether a slot index is within the bounds of the slots collection.
		/// </summary>
		/// <param name="slotIndex">The zero-based index of the slot to validate.</param>
		/// <returns><see langword="true"/> if <paramref name="slotIndex"/> is greater than or equal to 0 and less than the number of
		/// slots; otherwise, <see langword="false"/>.</returns>
		private bool IsValidSlotIndex(int slotIndex) => slotIndex >= 0 && slotIndex < this.slots.Length;

		/// <summary>
		/// Represents the state of an orbital slot in the registry.
		/// </summary>
		private enum SlotState
		{
			Free,
			Reserved,
			Occupied,
		}
	}

	/// <summary>
	/// Reservation handle for a specific orbital slot.
	/// </summary>
	public readonly struct OrbitalSlotReservation
	{
		/// <summary>
		/// Represents a reservation that does not refer to any valid orbital slot.
		/// </summary>
		public static OrbitalSlotReservation None => new(-1, -1);

		/// <summary>
		/// Initializes a new instance of the OrbitalSlotReservation class with the specified slot index and reservation
		/// token.
		/// </summary>
		/// <param name="slotIndex">Zero-based index of the orbital slot to reserve.</param>
		/// <param name="token">Token that identifies the reservation.</param>
		public OrbitalSlotReservation(int slotIndex, int token)
		{
			this.SlotIndex = slotIndex;
			this.Token = token;
		}

		public int SlotIndex { get; }

		public int Token { get; }

		/// <summary>
		/// Gets a value indicating whether the reservation is valid (i.e., it refers to a valid slot index and has a non-negative token).
		/// </summary>
		public bool IsValid => this.SlotIndex >= 0 && this.Token >= 0;
	}

	/// <summary>
	/// Occupancy handle for a specific orbital slot.
	/// </summary>
	public readonly struct OrbitalOccupiedSlot
	{
		public static OrbitalOccupiedSlot None => new(-1);

		/// <summary>
		/// Initializes a new instance of the OrbitalOccupiedSlot class with the specified slot index.
		/// </summary>
		/// <param name="slotIndex">Zero-based index of the orbital slot to occupy.</param>
		public OrbitalOccupiedSlot(int slotIndex)
		{
			this.SlotIndex = slotIndex;
		}

		public int SlotIndex { get; }

		/// <summary>
		/// Gets a value indicating whether the slot index is valid.
		/// </summary>
		public bool IsValid => this.SlotIndex >= 0;
	}
}
